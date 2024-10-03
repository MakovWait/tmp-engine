using System.Diagnostics;

namespace Tmp.Core.Redot;

internal interface IContextValue
{
    void HandleContextChanged<T>(T newVal);

    void Init();
    
    void Uninitialize();
}

public interface IContextValue<out T> : IEffectDependency
{
    T Get();
}

internal class ContextValue<T>(Node node) : IContextValue<T>, IContextValue
{
    public event Action? AboutToChange;
    public event Action? Changed;
    
    private T? value;
    private bool initialized;

    public T Get()
    {
        Debug.Assert(initialized, "Attempt to use uninitialized context value!!!");
        return value!;
    }
    
    public void HandleContextChanged<T1>(T1 newVal)
    {
        Debug.Assert(initialized);
        if (newVal is not T thisVal) return;
        AboutToChange?.Invoke();
        value = thisVal;
        Changed?.Invoke();
    }

    public void Init()
    {
        Debug.Assert(!initialized);
        value = node.FindInContext<T>();
        initialized = true;
    }

    public void Uninitialize()
    {
        Debug.Assert(initialized);
        initialized = false;
        value = default;
    }
}
