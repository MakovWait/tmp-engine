namespace Tmp.Core.Redot;

public interface IDeferredValueMut<T> : IDeferredValue<T>
{
	void Set(T val);
}

public interface IDeferredValue<out T> : IEffectDependency
{
	T Get();
}

public class DeferredValueAnonymous<T>(Func<T> value) : IDeferredValue<T>
{
	public T Get()
	{
		return value();
	}

	public event Action? AboutToChange;
	public event Action? Changed;
}

public class DeferredValue<T> : IDeferredValueMut<T>
{
	public event Action? AboutToChange;
	public event Action? Changed;

	private T? value;

	public void Set(T newValue)
	{
		AboutToChange?.Invoke();
		value = newValue;
		Changed?.Invoke();
	}

	public T Get()
	{
		return value!;
	}
}