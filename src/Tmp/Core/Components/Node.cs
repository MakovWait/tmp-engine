using System.Buffers;
using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Components;

public class CurrentScope
{
    private readonly Stack<IScope> _scopes = new();
    
    public void Push(IScope scope)
    {
        _scopes.Push(scope);
    }

    public void Pop()
    {
        _scopes.Pop();
    }
    
    public IScope? Value => _scopes.Count > 0 ? _scopes.Peek() : null;
}

public interface ICleanup
{
    public void Invoke();
}

public interface IScope
{
    public bool NoTrack { get; set; }
    
    void Bind(IScope child);
    
    void Bind(ICleanup cleanup);
    
    void Bind(ISignal signal);

    void Trigger();
    
    void Clean();
    
    void SetName(string name);
}

public interface IComputation
{
    public void Invoke();
}

public class Computation<T>(Func<T, T> fn, T initial) : IComputation
{
    public T Value { get; private set; } = initial;

    public void Invoke()
    {
        Value = fn(Value);
    }
}

[DebuggerDisplay("Name = {_name}")]
public class Scope(IComputation computation, CurrentScope scope) : IScope, ISignalTarget
{
    private string? _name;
    private readonly List<IScope> _children = [];
    private readonly List<ICleanup> _cleanups = [];
    private readonly List<ISignal> _signals = [];

    public bool NoTrack { get; set; } = false;

    public void Bind(IScope child)
    {
        if (_children.Contains(child)) return;
        _children.Add(child);
    }

    public void Bind(ICleanup cleanup)
    {
        _cleanups.Add(cleanup);
    }

    public void Bind(ISignal signal)
    {
        if (NoTrack) return;
        if (_signals.Contains(signal)) return;
        signal.Add(this);
        _signals.Add(signal);
    }

    public void Trigger()
    {
        Clean();
        
        scope.Push(this);
        computation.Invoke();
        scope.Pop();
    }

    void ISignalTarget.Invoke()
    {
        Trigger();
    }
    
    public void Clean()
    {
        foreach (var child in _children)
        {
            child.Clean();
        }
        _children.Clear();
        
        foreach (var cleanup in _cleanups)
        {
            cleanup.Invoke();
        }
        _cleanups.Clear();
        
        foreach (var signal in _signals)
        {
            signal.Remove(this);
        }
        _signals.Clear();
        _name = null;
    }

    public void SetName(string name)
    {
        _name = name;
    }
}

public interface ISignalScope
{
    void Bind(ISignal signal);
}

public interface ISignalTarget
{
    public void Invoke();
}

public class SignalTargets : IEnumerable<ISignalTarget>
{
    private readonly List<ISignalTarget> _targets = [];

    public void Add(ISignalTarget target)
    {
        _targets.Add(target);
    }

    public bool Contains(ISignalTarget target)
    {
        return _targets.Contains(target);
    }
    
    public void Remove(ISignalTarget target)
    {
        _targets.Remove(target);
    }

    public void Clear()
    {
        _targets.Clear();
    }

    public void Invoke()
    {
        var count = _targets.Count;
        var targetsCopy = ArrayPool<ISignalTarget>.Shared.Rent(count);
        _targets.CopyTo(targetsCopy);
        
        try
        {
            for (var i = 0; i < count; i++)
            {
                targetsCopy[i].Invoke();
            }
        }
        finally
        {
            ArrayPool<ISignalTarget>.Shared.Return(targetsCopy);
        }
    }
    
    public void InvokeBatched(ISignalBatch batch)
    {
        foreach (var target in _targets)
        {
            batch.Add(target);
        }
    }
    
    public IEnumerator<ISignalTarget> GetEnumerator()
    {
        return _targets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class SignalBatch : ISignalBatch
{
    private readonly SignalTargets _targets = [];
    
    public void Add(ISignalTarget target)
    {
        if (_targets.Contains(target)) return;
        _targets.Add(target);
    }

    public void Trigger()
    {
        _targets.Invoke();
        _targets.Clear();
    }
}

public interface ISignalBatch
{
    void Add(ISignalTarget target);
}

public interface ISignal
{
    void Add(ISignalTarget target);
    
    void Remove(ISignalTarget target);
}

public class Signals(CurrentScope scope, SignalBatch targetsBatchedQueue)
{
    private readonly CurrentSignalScope _currentSignalScope = new(scope);
    private readonly Config _config = new();
    private bool _batched;

    public bool Batched
    {
        get => _batched;
        set => SetBatched(value);
    }

    public Signal<T> Create<T>(T initial)
    {
        return new Signal<T>(initial, _currentSignalScope, _config, targetsBatchedQueue);
    }

    public void FlushBatches()
    {
        targetsBatchedQueue.Trigger();
    }

    private void SetBatched(bool value)
    {
        _batched = value;
        _config.Batched = value;
    }
    
    private class CurrentSignalScope(CurrentScope scope) : ISignalScope
    {
        public void Bind(ISignal signal)
        {
            scope.Value?.Bind(signal);
        }
    }

    private class Config : ISignalConfig
    {
        public bool Batched { get; set; } = false;
    }
}

public interface ISignalConfig
{
    public bool Batched { get; }
}

public class Signal<T>
{
    private T _value;
    private readonly InnerSignal _innerSignal;
    private string? _name;
    private readonly ISignalScope _scope;

    public Signal(T initial, ISignalScope scope, ISignalConfig config, ISignalBatch batch)
    {
        _scope = scope;
        _value = initial;
        _innerSignal = new InnerSignal(this, config, batch);
    }

    public T Value
    {
        get => Get();
        set => Set(value);
    }

    public Signal<T> WithName(string name)
    {
        _name = name;
        return this;
    }

    private T Get()
    {
        _scope.Bind(_innerSignal);
        return _value;
    }

    private void Set(T value)
    {
        _value = value;
        _innerSignal.Notify();
    }
    
    private class InnerSignal(Signal<T> signal, ISignalConfig config, ISignalBatch batch) : ISignal
    {
        private readonly SignalTargets _targets = [];
        private readonly Signal<T> _signal = signal;

        public void Notify()
        {
            if (config.Batched)
            {
                _targets.InvokeBatched(batch);
            }
            else
            {
                _targets.Invoke();
            }
        }
        
        public void Add(ISignalTarget target)
        {
            _targets.Add(target);
        }

        public void Remove(ISignalTarget target)
        {
            _targets.Remove(target);
        }
    }
}

public class Cleanup(Action action) : ICleanup
{
    public void Invoke()
    {
        action();
    }
}

public class BatchedComputation(Node batchSource, IComputation origin) : IComputation
{
    public void Invoke()
    {
        batchSource.Batch(origin.Invoke);
    }
}

public readonly struct Unit;

public class Node(Tree tree, CurrentScope currentScope, Signals signals)
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

    public void CreateChildren(IEnumerable<IComponent> components)
    {
        Debug.Assert(_state == NodeState.Mounted);
        foreach (var component in components)
        {
            CreateChild(component);
        }
    }
    
    public void ReplaceChildren(IEnumerable<IComponent> components)
    {
        Debug.Assert(_state == NodeState.Mounted);
        RemoveChildren();
        CreateChildren(components);
    }
    
    public void ReplaceChildren(IComponent component)
    {
        Debug.Assert(_state == NodeState.Mounted);
        RemoveChildren();
        CreateChild(component);
    }
    
    public void RemoveChildren()
    {
        for (var i = _children.Count - 1; i >= 0; i--)
        {
            var child = _children[i];
            child.Free();
            RemoveChild(child);
        }
    }
    
    public void CreateChild(IComponent component)
    {
        Debug.Assert(_state == NodeState.Mounted);
        var child = CreateUnmountedChild(component);
        child.Mount();
    }

    private Node CreateUnmountedChild(IComponent component)
    {
        var child = tree.CreateNode();
        AddChild(child);
        foreach (var childComponent in component.Build(child))
        {
            child.CreateUnmountedChild(childComponent);
        }
        return child;
    }

    public void BuildAsRoot(IComponent component)
    {
        foreach (var childComponent in component.Build(this))
        {
            CreateUnmountedChild(childComponent);
        }

        Mount();
    }
    
    public void Build(Action<Node> build)
    {
        _scope = new Scope(
            new Computation<Unit>(prev =>
            {
                build(this);
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
    
    public IEnumerable<IComponent> Build(Func<Node, IEnumerable<IComponent>> component)
    {
        var computation = new Computation<IEnumerable<IComponent>>(_ => component.Invoke(this), []); 
        _scope = new Scope(
            new BatchedComputation(this, computation),
            currentScope
        )
        {
            NoTrack = true
        };
        // currentScope.Value?.Bind(_scope);
        _scope.Trigger();
        return computation.Value;
    }
    
    private void RegisterScope(IScope scope)
    {
        currentScope.Value?.Bind(scope);
    }

    public Signal<T> CreateSignal<T>(T initial)
    {
        return signals.Create(initial);
    }

    public void OnCleanup(ICleanup cleanup)
    {
        currentScope.Value?.Bind(cleanup);
    }

    public void OnMount(Action action)
    {
        UseEffect(() => Untrack(action));
    }
    
    public void UseEffect(IComputation computation)
    {
        var effectScope = new Scope(
            new BatchedComputation(this, computation),
            // computation,
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

    public Signal<T> UseMemo<T>(Func<T, T> fn, T initial)
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

    public void Untrack(Action action)
    {
        var scope = currentScope.Value!;
        var prev = scope.NoTrack;
        scope.NoTrack = true;
        try
        {
            action();
        }
        finally
        {
            scope.NoTrack = prev;
        }
    }

    public void Batch(Action action)
    {
        if (signals.Batched)
        {
            action();
        }
        else
        {
            var prev = signals.Batched; 
            signals.Batched = true;
            try
            {
                action();
            }
            finally
            {
                signals.FlushBatches();
                signals.Batched = prev;
            }
        }
    }
    
    public void UseEffect<T>(Func<T, T> effect, T initial)
    {
        UseEffect(new Computation<T>(effect, initial));
    }
    
    public void UseEffect(Action effect)
    {
        UseEffect(prev =>
        {
            effect();
            return prev;
        }, new Unit());
    }

    public void SetScopeName(string name)
    {
        currentScope.Value?.SetName(name);
    }
}

public interface IComponent : IEnumerable<IComponent>
{
    IEnumerable<IComponent> Build(Node node);
}

public class Tree
{
    private readonly CurrentScope _currentScope;
    private readonly Signals _signals;
    private readonly Node _root;
    
    public bool IsBuilding { get; private set; }

    public Tree(CurrentScope currentScope, Signals signals)
    {
        _currentScope = currentScope;
        _signals = signals;
        _root = CreateNode();
        IsBuilding = true;
    }
    
    public Tree(CurrentScope currentScope) : this(currentScope, new Signals(currentScope, new SignalBatch()))
    {
    }
    
    public Tree() : this(new CurrentScope())
    {
    }

    public void Build(IComponent component)
    {
        Debug.Assert(IsBuilding);
        _root.BuildAsRoot(component);
        IsBuilding = false;
    }

    internal Node CreateNode()
    {
        return new Node(this, _currentScope, _signals);
    }
}

public abstract class Component : IComponent, IEnumerable<IComponent>
{
    public List<IComponent> Children { get; init; } = [];
    
    // protected IReadOnlyList<IComponent> Children => _childComponents;
    
    public void Add(IComponent component)
    {
        Children.Add(component);
    }

    IEnumerable<IComponent> IComponent.Build(Node node)
    {
        return node.Build(Build);
    }

    protected abstract IEnumerable<IComponent> Build(Node node);

    public IEnumerator<IComponent> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class ComponentFunc(Func<Node, IEnumerable<IComponent>> func) : Component
{
    protected override IEnumerable<IComponent> Build(Node node)
    {
        return func(node);
    }
}

public class Conditional(Signal<bool> cond) : Component
{
    protected override IEnumerable<IComponent> Build(Node self)
    {
        self.UseEffect(() =>
        {
            var c = cond.Value;
            self.Untrack(() =>
            {
                if (c)
                {
                    self.CreateChildren(Children);
                }
                else
                {
                    self.RemoveChildren();
                }
            });
        });
            
        return [];
    }
}

public static class ComponentEx
{
    public static Conditional If(this IComponent self, Signal<bool> cond)
    {
        return new Conditional(cond)
        {
            self
        };
    }
}