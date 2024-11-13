using System.Diagnostics;

namespace Tmp.Core.Comp;

[DebuggerDisplay("{NodePath}")]
public class Node : INodeInit
{
    private Node? _parent;
    private readonly Context _ctx = new();
    private readonly Children _children = new();
    private readonly Callbacks _callbacks = new();
    private readonly Callbacks _lateCallbacks = new();
    
    private NodeState _state = NodeState.Building;
    private Scope? _scope;
    private readonly Tree _tree;
    private readonly CurrentScope _currentScope;
    private readonly Signals _signals;
    private readonly ISignalMut<string> _name;

    public ISignal<string> Name => _name;

    public Node(string initialName, Tree tree, CurrentScope currentScope, Signals signals)
    {
        _tree = tree;
        _currentScope = currentScope;
        _signals = signals;
        _name = signals.Create(initialName, new SignalValueEquals.Equitable<string>());
    }

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
        _children.Mount();
    }

    public void Free()
    {
        _scope!.Clean();
        _children.Free();
        _state = NodeState.Freed;
    }

    public Node? GetNode(NodePath nodePath)
    {
        Node? current = null;
        if (!nodePath.IsAbsolute())
        {
            current = this;
        }
        
        foreach (var name in nodePath.Names())
        {
            Node? next = null;
            if (name == ".")
            {
                next = current;
            }
            else if (name == "..")
            {
                if (current?._parent is null)
                {
                    return null;
                }

                next = current._parent;
            }
            else if (current == null)
            {
                if (name.Equals(_tree.Root.Name.UntrackedValue))
                {
                    next = _tree.Root;
                }
            }
            else
            {
                next = null;
                var node = current._children.GetByName(name);
                if (node != null)
                {
                    next = node;
                }
                else
                {
                    return null;
                }
            }
            current = next;
        }

        return current;
    }
    
    public void AddChild(Node child)
    {
        Debug.Assert(child._parent is null);
        child._parent = this;
        _children.Add(child);
        ValidateChildName(child);
    }
    
    public void RemoveChild(Node child)
    {
        Debug.Assert(child._parent == this);
        child._parent = null;
        _children.Remove(child);
    }
    
    public void ClearChildren()
    {
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
            new Computation<NoArgs>(prev =>
            {
                init(this);
                return prev;
            }, new NoArgs()),
            _currentScope,
            _tree
        )
        {
            NoTrack = true
        };
        // currentScope.Value?.Bind(_scope);
        _scope.Trigger();
    }

    private void ValidateChildName(Node child)
    {
        var name = child.Name.UntrackedValue;
        var unique = _children.NameIsUnique(child);
        if (unique) return;
        child._name.Value = $"@{name}@{_children.IndexOf(child)}";
    }
    
    private void RegisterScope(IScope scope)
    {
        _currentScope.Value?.Bind(scope);
    }

    public ISignalMut<T> UseSignal<T>(T initial, ISignalValueEquals<T>? equals)
    {
        return _signals.Create(initial, equals);
    }
    
    public ISignal<T> CreateSignal<T>(CreateSignalFactory<T> factory, T initial)
    {
        var signal = _signals.Create(initial, null);
        
        // TODO add cleanup to signals
        var cleanup = factory(newVal =>
        {
            signal.Value = newVal;
        });
        
        return signal;
    }

    public void OnCleanup(ICleanup cleanup)
    {
        _currentScope.Value?.Bind(cleanup);
    }

    public void OnMount(Action action)
    {
        this.UseEffect(() => Untrack(a => a(), action));
    }
    
    public void UseEffect(IComputation computation)
    {
        var effectScope = new Scope(
            new BatchedComputation(this, computation),
            _currentScope,
            _tree
        )
        {
            Deferred = true
        };
        _tree.Enqueue(effectScope);
        RegisterScope(effectScope);
    }

    public void UseComputed(IComputation computation)
    {
        var scope = new Scope(computation, _currentScope, _tree);
        RegisterScope(scope);
        scope.Trigger();
    }

    public ISignal<T> UseMemo<T>(Func<T, T> fn, T initial, ISignalValueEquals<T>? equals)
    {
        var signal = UseSignal(initial, equals);
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
            _currentScope,
            _tree
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
        var scope = _currentScope.Value!;
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
        if (_signals.Batched)
        {
            action(args);
        }
        else
        {
            var prev = _signals.Batched; 
            _signals.Batched = true;
            try
            {
                action(args);
            }
            finally
            {
                _signals.FlushBatches();
                _signals.Batched = prev;
            }
        }
    }
    
    public void SetScopeName(string name)
    {
        _currentScope.SetName(name);
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

    public void SetName(string value)
    {
        Batch(args =>
        {
            var (self, val) = args;
            self._name.Value = val;
            self._parent?.ValidateChildName(self);
        }, (this, value));
    }

    public void CallDeferred<T>(Action<T> action, T args)
    {
        _tree.CallDeferred(action, args);
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
    
    private string NodePath => string.Join("/", _parent?.NodePath ?? "", Name.Value);
    
    private class Children
    {
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
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

        public int IndexOf(Node node)
        {
            return List.IndexOf(node);
        }

        public bool NameIsUnique(Node child)
        {
            foreach (var node in List)
            {
                if (node == child) continue;
                if (node.Name.UntrackedValue.Equals(child.Name.UntrackedValue))
                {
                    return false;
                }
            }
            return true;
        }

        public Node? GetByName(string name)
        {
            foreach (var node in List)
            {
                if (node.Name.UntrackedValue.Equals(name))
                {
                    return node;
                }
            }
            return null;
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

[DebuggerDisplay("{AsString}")]
public sealed class NodePath(string path) : IEquatable<string>
{
    public bool IsAbsolute()
    {
        return path.StartsWith('/');
    }

    public IReadOnlyList<string> Names()
    {
        return path.Split('/').Where(x => !x.Equals("")).ToList();
    }

    public bool Equals(string? other)
    {
        return path.Equals(other);
    }
    
    private string AsString => path;
}

public interface IDeferredAction
{
    void Invoke();
}

public class DeferredActionWithArgs<T>(Action<T> action, T args) : IDeferredAction
{
    public void Invoke()
    {
        action(args);
    }
}

public class DeferredQueue : IDeferredQueue
{
    private readonly List<IDeferredAction> _actions = [];
    
    public void Enqueue(IDeferredAction action)
    {
        if (_actions.Contains(action)) return;
        _actions.Add(action);
    }

    public void Flush()
    {
        foreach (var action in _actions)
        {
            action.Invoke();
        }
        _actions.Clear();
    }
}

public interface IDeferredQueue
{
    public void Enqueue(IDeferredAction deferredAction);
}

public class Tree : IDeferredQueue
{
    private readonly CurrentScope _currentScope;
    private readonly Signals _signals;
    private readonly TreeDeferredQueue _deferredQueue = new();
    private Node? _root;

    internal Node Root => _root!;
    
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
        _root!.Call(args);
    }
    
    public void Call<T>() where T : new()
    {
        _root!.Call(new T());
    }

    public void Free()
    {
        _root!.Free();
    }
    
    public void Build(IComponent component)
    {
        _root = component.Build(this, null);
        _root.Mount();
    }

    public void Build(Node root)
    {
        _root = root;
        _root.Mount();
    }

    public void FlushDeferredQueue()
    {
        _deferredQueue.Flush();
    }

    public void Enqueue(IDeferredAction action)
    {
        _deferredQueue.Enqueue(action);
    }
    
    public void CallDeferred<T>(Action<T> action, T args)
    {
        // TODO pool
        Enqueue(new DeferredActionWithArgs<T>(action, args));
    }

    internal Node CreateNode(string name)
    {
        return new Node(name, this, _currentScope, _signals);
    }

    private class TreeDeferredQueue
    {
        private readonly Queue<DeferredQueue> _buffer = new();
        private DeferredQueue _current;

        public TreeDeferredQueue()
        {
            _current = new DeferredQueue();
            _buffer.Enqueue(_current);
            _buffer.Enqueue(new DeferredQueue());
        }

        public void Enqueue(IDeferredAction action)
        {
            _current.Enqueue(action);
        }

        private void ChangeBuffer()
        {
            _current = _buffer.Dequeue();
            _buffer.Enqueue(_current);
        }
        
        public void Flush()
        {
            var bufferToFlush = _current;
            ChangeBuffer();
            bufferToFlush.Flush();
        }
    }
}

public delegate Action CreateSignalFactory<out T>(Action<T> set);

public interface INodeInit
{
    ISignal<string> Name { get; }
    
    void UseEffect(IComputation computation);
    
    void UseComputed(IComputation computation);

    ISignal<T> UseMemo<T>(Func<T, T> fn, T initial, ISignalValueEquals<T>? equals);

    ISignalMut<T> UseSignal<T>(T initial, ISignalValueEquals<T>? equals);

    ISignal<T> CreateSignal<T>(CreateSignalFactory<T> factory, T initial);
    
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

    void SetName(string value);

    void CallDeferred<T>(Action<T> action, T args);
}

public static class NodeInitExUseEquitableSignal
{
    public static ISignalMut<T> UseSignal<T>(this INodeInit self, T initial) where T : IEquatable<T>
    {
        return self.UseSignal(initial, new SignalValueEquals.Equitable<T>());
    }
    
    public static ISignal<T> UseMemo<T>(this INodeInit self, Func<T, T> fn, T initial) where T : IEquatable<T>
    {
        return self.UseMemo(fn, initial, new SignalValueEquals.Equitable<T>());
    }
}

public static class NodeInitExUseObjSignal
{
    public static ISignal<T> UseMemo<T>(this INodeInit self, Func<T, T> fn, T initial) where T : class
    {
        return self.UseMemo(fn, initial, new SignalValueEquals.ObjRef<T>());
    }
    
    public static ISignalMut<T> UseSignal<T>(this INodeInit self, T initial) where T : class
    {
        return self.UseSignal(initial, new SignalValueEquals.ObjRef<T>());
    }
}

public static class NodeInitEx
{
    public static void Call<T>(this INodeInit self) where T : new()
    {
        self.Call<T>(new T());
    }
    
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
        }, new NoArgs());
    }
    
    public static void UseComputed<T>(this INodeInit self, Func<T, T> effect, T initial)
    {
        self.UseComputed(new Computation<T>(effect, initial));
    }
    
    public static void UseComputed(this INodeInit self, Action effect)
    {
        self.UseComputed(prev =>
        {
            effect();
            return prev;
        }, new NoArgs());
    }
    
    public static void OnCleanup(this INodeInit self, Action cleanup)
    {
        self.OnCleanup(new Cleanup(cleanup));
    }
}
