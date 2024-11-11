using System.Diagnostics;

namespace Tmp.Core.Comp;

public class Node(Tree tree, CurrentScope currentScope, Signals signals) : INodeInit
{
    private Node? _parent;
    private readonly List<Node> _children = [];

    private NodeState _state = NodeState.Building;
    private Scope? _scope;
    private readonly Queue<IScope> _onMountScopes = [];

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

        foreach (var child in _children)
        {
            child.Mount();
        }
    }

    public void Free()
    {
        _scope!.Clean();

        foreach (var child in _children)
        {
            child.Free();
        }

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
        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
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
