using System.Diagnostics;

namespace Tmp.Core.Comp;

[DebuggerDisplay("Name = {_name}")]
public class Scope(IComputation computation, CurrentScope scope) : IScope, ISignalTarget
{
    private string? _name;
    private readonly List<IScope> _children = [];
    private readonly List<ICleanup> _cleanups = [];
    private readonly List<IObservableSignal> _signals = [];

    public bool NoTrack { get; set; } = false;

    public string? Name { get; set; }

    public void Bind(IScope child)
    {
        if (_children.Contains(child)) return;
        _children.Add(child);
    }

    public void Bind(ICleanup cleanup)
    {
        _cleanups.Add(cleanup);
    }

    public void Bind(IObservableSignal signal)
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
}

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

    public void SetName(string name)
    {
        if (Value != null)
        {
            Value.Name = name;
        } 
    }
    
    public IScope? Value => _scopes.Count > 0 ? _scopes.Peek() : null;
}

public interface ICleanup
{
    public void Invoke();
}

public class Cleanup(Action action) : ICleanup
{
    public void Invoke()
    {
        action();
    }
}

public interface IScope
{
    public bool NoTrack { get; set; }
    
    public string? Name { get; set; }

    void Bind(IScope child);

    void Bind(ICleanup cleanup);

    void Bind(IObservableSignal signal);

    void Trigger();

    void Clean();
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

public class BatchedComputation(Node batchSource, IComputation origin) : IComputation
{
    public void Invoke()
    {
        batchSource.Batch(o => o.Invoke(), origin);
    }
}

public readonly struct Unit;