using System.Diagnostics;

namespace Tmp.Core.Comp;

public class Node(Tree tree, CurrentScope currentScope, Signals signals) : INodeInit
{
    private Node? _parent;
    private readonly Context _ctx = new();
    private readonly Children _children = new();
    private readonly Callbacks _callbacks = new();
    private readonly Callbacks _lateCallbacks = new();

    private readonly Queue<IScope> _onMountScopes = [];
    
    private NodeState _state = NodeState.Building;
    private Scope? _scope;

    enum NodeState
    {
        Building,
        Mounted,
        QueuedForDeletion,
        Freed,
    }
    
    public void Mount()
    {
        _state = NodeState.Mounted;
        while (_onMountScopes.Count > 0)
        {
            _onMountScopes.Dequeue().Trigger();
        }
        _children.Mount();
    }

    public void Free()
    {
        _scope!.Clean();
        _children.Free();
        _state = NodeState.Freed;
    }
    
    public void AddChild(Node child)
    {
        Debug.Assert(child._parent is null);
        child._parent = this;
        _children.Add(child);
    }
    
    public void RemoveChild(Node child)
    {
        Debug.Assert(child._parent == this);
        child._parent = null;
        _children.Remove(child);
    }
    
    public void ClearChildren()
    {
        _children.Clear();
        
        for (var i = _children.List.Count - 1; i >= 0; i--)
        {
            var child = _children.List[i];
            RemoveChild(child);
            child.Free();
        }
    }
    
    public void Init(Action<Node> init)
    {
        _scope = new Scope(
            new Computation<Unit>(prev =>
            {
                init(this);
                return prev;
            }, new Unit()),
            currentScope
        )
        {
            NoTrack = true
        };
        currentScope.Value?.Bind(_scope);
        _scope.Trigger();
    }
    
    private void RegisterScope(IScope scope)
    {
        currentScope.Value?.Bind(scope);
    }

    public ISignalMut<T> CreateSignal<T>(T initial)
    {
        return signals.Create(initial);
    }

    public void OnCleanup(ICleanup cleanup)
    {
        currentScope.Value?.Bind(cleanup);
    }

    public void OnMount(Action action)
    {
        this.UseEffect(() => Untrack(a => a(), action));
    }
    
    public void UseEffect(IComputation computation)
    {
        var effectScope = new Scope(
            new BatchedComputation(this, computation),
            currentScope
        );
        RegisterScope(effectScope);
        if (_state == NodeState.Building)
        {
            _onMountScopes.Enqueue(effectScope);
        }
        else
        {
            effectScope.Trigger();
        }
    }

    public ISignal<T> UseMemo<T>(Func<T, T> fn, T initial)
    {
        var signal = CreateSignal(initial);
        var memoScope = new Scope(
            new BatchedComputation(
                this, 
                new Computation<T>(prev =>
                {
                    var next = fn(prev);
                    signal.Value = next;
                    return next;
                }, initial)
            ),
            currentScope
        );
        memoScope.Trigger();
        RegisterScope(memoScope);
        return signal;
    }
    
    public void OnCleanup(Action cleanup)
    {
        OnCleanup(new Cleanup(cleanup));
    }

    public void Untrack<TArgs>(Action<TArgs> action, TArgs args)
    {
        var scope = currentScope.Value!;
        var prev = scope.NoTrack;
        scope.NoTrack = true;
        try
        {
            action(args);
        }
        finally
        {
            scope.NoTrack = prev;
        }
    }

    public void Batch<TArgs>(Action<TArgs> action, TArgs args)
    {
        if (signals.Batched)
        {
            action(args);
        }
        else
        {
            var prev = signals.Batched; 
            signals.Batched = true;
            try
            {
                action(args);
            }
            finally
            {
                signals.FlushBatches();
                signals.Batched = prev;
            }
        }
    }
    
    public void SetScopeName(string name)
    {
        currentScope.Value?.SetName(name);
    }

    public T CreateContext<T>(T value)
    {
        _ctx.Create(value);
        return value;
    }

    public T UseContext<T>()
    {
        return FindInContext<T>();
    }

    public void Call<T>(T args)
    {
        _callbacks.Call(args);
        _children.Call(args);
        _lateCallbacks.Call(args);
    }

    public void On<T>(Action<T> action)
    {
        _callbacks.Add(action);
    }

    public void OnLate<T>(Action<T> action)
    {
        _lateCallbacks.Add(action);
    }

    private T FindInContext<T>()
    {
        if (_parent == null)
        {
            throw new Exception($"Unable to find a context value for {typeof(T)}");
        }

        if (_parent._ctx.Has<T>())
        {
            return _parent._ctx.Get<T>();
        }
        
        return _parent!.FindInContext<T>();
    }
    
    private class Children
    {
        public readonly List<Node> List = [];

        public void Add(Node child)
        {
            List.Add(child);
        }

        public void Remove(Node child)
        {
            List.Remove(child);
        }
        
        public void Call<T>(T state)
        {
            foreach (var child in List)
            {
                child.Call(state);
            }
        }

        public void Free()
        {
            foreach (var child in List)
            {
                child.Free();
            }
        }

        public void Mount()
        {
            foreach (var child in List)
            {
                child.Mount();
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
    
    private class Context
    {
        private readonly Dictionary<Type, object> _map = new();

        public T Get<T>()
        {
            return (T)_map[typeof(T)];
        }

        public bool Has<T>()
        {
            return _map.ContainsKey(typeof(T));
        }
    
        public void Create<T>(T val)
        {
            if (val == null)
            {
                throw new ArgumentNullException(nameof(val));
            }
            
            _map[typeof(T)] = val;
        }
    }
    
    internal class Callbacks
    {
        private readonly List<ICallback> _callbacks = [];
    
        public void Call<T>(T state)
        {
            foreach (var callback in _callbacks)
            {
                callback.Handle(state);
            }
        }

        public void Add<T>(Action<T> callback)
        {
            _callbacks.Add(new Callback<T>(callback));
        }

        private interface ICallback
        {
            void Handle<T>(T state);
        }
        
        private class Callback<Y>(Action<Y> callback) : ICallback
        {
            public void Handle<T>(T state)
            {
                if (state is Y casted)
                {
                    callback(casted);
                }
            }
        }
    }
}

public class Tree
{
    private readonly CurrentScope _currentScope;
    private readonly Signals _signals;
    private Node? _root;
    
    public Tree(CurrentScope currentScope, Signals signals)
    {
        _currentScope = currentScope;
        _signals = signals;
    }
    
    public Tree(CurrentScope currentScope) : this(currentScope, new Signals(currentScope, new SignalBatch()))
    {
    }
    
    public Tree() : this(new CurrentScope())
    {
    }

    public void Call<T>(T args)
    {
        _root.Call(args);
    }
    
    public void Build(IComponent component)
    {
        _root = component.Build(this, null);
        _root.Mount();
    }

    internal Node CreateNode()
    {
        return new Node(this, _currentScope, _signals);
    }
}

public interface INodeInit
{
    void UseEffect(IComputation computation);

    ISignal<T> UseMemo<T>(Func<T, T> fn, T initial);

    ISignalMut<T> CreateSignal<T>(T initial);

    void OnCleanup(ICleanup cleanup);

    void OnMount(Action action);

    void Untrack<TArgs>(Action<TArgs> action, TArgs args);

    void Batch<TArgs>(Action<TArgs> action, TArgs args);

    void SetScopeName(string name);

    T CreateContext<T>(T value);
    
    T UseContext<T>();

    void Call<T>(T args);
    
    void On<T>(Action<T> action);
    
    void OnLate<T>(Action<T> action);
}

public static class NodeInitEx
{
    public static void UseEffect<T>(this INodeInit self, Func<T, T> effect, T initial)
    {
        self.UseEffect(new Computation<T>(effect, initial));
    }
    
    public static void UseEffect(this INodeInit self, Action effect)
    {
        self.UseEffect(prev =>
        {
            effect();
            return prev;
        }, new Unit());
    }
    
    public static void OnCleanup(this INodeInit self, Action cleanup)
    {
        self.OnCleanup(new Cleanup(cleanup));
    }
}
