using System.Buffers;
using System.Collections;
using System.Diagnostics;

namespace Tmp.Core.Comp;

[DebuggerDisplay("Name = {_name}, Value = {_value}")]
internal class Signal<T> : ISignalMut<T>
{
    private T _value;
    private readonly InnerSignal _innerSignal;
    private string? _name;
    private readonly ISignalScope _scope;
    private ISignalValueEquals<T>? _equals;

    public Signal(T initial, ISignalScope scope, ISignalConfig config, ISignalBatch batch, ISignalValueEquals<T>? equals)
    {
        _scope = scope;
        _equals = equals;
        _value = initial;
        _innerSignal = new InnerSignal(this, config, batch);
    }

    public T Value
    {
        get => Get();
        set => Set(value);
    }

    public T UntrackedValue
    {
        get => _value;
        set => _value = value;
    }

    public void SetValueEquals(ISignalValueEquals<T>? equals)
    {
        _equals = equals;
    }

    public void SetDebugName(string name)
    {
        _name = name;
    }

    private T Get()
    {
        _scope.Bind(_innerSignal);
        return _value;
    }

    private void Set(T value)
    {
        if (_equals?.Equals(_value, value) ?? false) return;
        _value = value;
        _innerSignal.Notify();
    }

    private class InnerSignal(Signal<T> signal, ISignalConfig config, ISignalBatch batch) : IObservableSignal
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

public interface ISignalValueEquals<in T> {
    bool Equals(T prev, T next);
}

public static class SignalValueEquals
{
    public class ObjEquals<TValue> : ISignalValueEquals<TValue> where TValue : class
    {
        public bool Equals(TValue prev, TValue next)
        {
            return prev.Equals(next);
        }
    }
    
    public class ObjRef<TValue> : ISignalValueEquals<TValue> where TValue : class
    {
        public bool Equals(TValue prev, TValue next)
        {
            return prev == next;
        }
    }
    
    public class Equitable<TValue> : ISignalValueEquals<TValue> where TValue : IEquatable<TValue>
    {
        public bool Equals(TValue prev, TValue next)
        {
            return prev.Equals(next);
        }
    }
    
    public class Const<TValue>(bool result) : ISignalValueEquals<TValue>
    {
        public bool Equals(TValue prev, TValue next)
        {
            return result;
        }
    }
}

public interface ISignal
{
    void SetDebugName(string name);
}

public interface ISignal<out T> : ISignal
{
    public T Value { get; }
    
    public T UntrackedValue { get; }

    void SetValueEquals(ISignalValueEquals<T>? equals);
}

[DebuggerDisplay("Name = {_name}, Value = {Value}")]
public class SignalConst<T>(T value) : ISignal<T>
{
    private string? _name;

    public void SetDebugName(string name)
    {
        _name = name;
    }

    public T Value { get; } = value;

    public T UntrackedValue => Value;

    public void SetValueEquals(ISignalValueEquals<T>? equals)
    {
        
    }
}

public interface ISignalMut<T> : ISignal<T>
{
    public new T Value { get; set; }
    
    public new T UntrackedValue { get; set; }
}

public static class SignalEx
{
    public static ISignal<T> ToSignal<T>(this T value)
    {
        return new SignalConst<T>(value);
    }
    
    public static TSelf WithName<TSelf>(this TSelf self, string name) where TSelf : ISignal
    {
        self.SetDebugName(name);
        return self;
    }
    
    public static TSelf WithEquals<T, TSelf>(this TSelf self, ISignalValueEquals<T>? equals) where TSelf : ISignal<T>
    {
        self.SetValueEquals(equals);
        return self;
    }
    
    public static TSelf Set<T, TSelf>(this TSelf self, T value) where TSelf : ISignalMut<T>
    {
        self.Value = value;
        return self;
    }
    
    public static T Get<T>(this ISignal<T> self)
    {
        return self.Value;
    }
    
    public static void Track<T>(this ISignal<T> self)
    {
        self.Get();
    }
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

    public ISignalMut<T> Create<T>(T initial, ISignalValueEquals<T>? equals)
    {
        return new Signal<T>(initial, _currentSignalScope, _config, targetsBatchedQueue, equals);
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
        public void Bind(IObservableSignal signal)
        {
            scope.Value?.Bind(signal);
        }
    }

    private class Config : ISignalConfig
    {
        public bool Batched { get; set; } = false;
    }
}

public interface IObservableSignal
{
    void Add(ISignalTarget target);

    void Remove(ISignalTarget target);
}

public interface ISignalConfig
{
    public bool Batched { get; }
}

public interface ISignalBatch
{
    void Add(ISignalTarget target);
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

    public List<ISignalTarget>.Enumerator GetEnumerator()
    {
        return _targets.GetEnumerator();
    }
    
    IEnumerator<ISignalTarget> IEnumerable<ISignalTarget>.GetEnumerator()
    {
        return _targets.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public interface ISignalScope
{
    void Bind(IObservableSignal signal);
}

public interface ISignalTarget
{
    public void Invoke();
}