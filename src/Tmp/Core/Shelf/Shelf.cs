using System.Diagnostics;

namespace Tmp.Core.Shelf;

public sealed class Shelf : IShelf
{
	private readonly Dictionary<Type, object> registry = new();
	private readonly Signals setSignals = new();
	private readonly Signals delSignals = new();

	public void Set<T>(T value)
	{
		registry[typeof(T)] = value ?? throw new ArgumentNullException(nameof(value));
		setSignals.Notify(value);
	}

	public T Get<T>()
	{
		Debug.Assert(Has<T>());
		return (T)registry[typeof(T)];
	}

	public bool Has<T>()
	{
		return registry.ContainsKey(typeof(T));
	}

	public void Del<T>()
	{
		Debug.Assert(Has<T>());
		var value = Get<T>();
		registry.Remove(typeof(T));
		delSignals.Notify(value);
	}

	public IDisposable OnDel<T>(Action<T> callback)
	{
		return delSignals.Add(callback);
	}

	public IDisposable OnSet<T>(Action<T> callback)
	{
		return setSignals.Add(callback);
	}
}

internal sealed class Signals
{
	private readonly EffectsRegistry registry = new();

	public void Notify<T>(T what)
	{
		foreach (var effect in registry.Get<T>())
		{
			effect.Notify(what);
		}
	}

	public IDisposable Add<T>(Action<T> callback)
	{
		var effect = new Effect<T>(callback);
		registry.Add<T>(effect);
		return new DeleteEffect(() => registry.Remove<T>(effect));
	}

	private sealed class EffectsRegistry
	{
		private readonly Dictionary<Type, IList<IEffect>> effects = [];

		public IEnumerable<IEffect> Get<T>()
		{
			return Bucket<T>();
		}

		public void Add<T>(IEffect effect)
		{
			Bucket<T>().Add(effect);
		}

		public void Remove<T>(IEffect effect)
		{
			Bucket<T>().Remove(effect);
		}

		private IList<IEffect> Bucket<T>()
		{
			if (!effects.ContainsKey(typeof(T)))
			{
				effects[typeof(T)] = [];
			}

			return effects[typeof(T)];
		}
	}

	private sealed class DeleteEffect(Action action) : IDisposable
	{
		public void Dispose()
		{
			action();
		}
	}

	private interface IEffect
	{
		void Notify<T>(T what);
	}

	private sealed class Effect<T>(Action<T> callback) : IEffect
	{
		public void Notify<T1>(T1 what)
		{
			if (what is T what1)
			{
				callback(what1);
			}
		}
	}
}
