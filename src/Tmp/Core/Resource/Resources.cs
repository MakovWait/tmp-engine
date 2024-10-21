using System.Diagnostics;

namespace Tmp.Core.Resource;

public sealed class Resources : IResources
{
	private readonly Dictionary<Type, object> _registry = new();
	private readonly Signals _setSignals = new();
	private readonly Signals _delSignals = new();

	public void Set<T>(T value)
	{
		_registry[typeof(T)] = value ?? throw new ArgumentNullException(nameof(value));
		_setSignals.Notify(value);
	}

	public T Get<T>()
	{
		Debug.Assert(Has<T>());
		return (T)_registry[typeof(T)];
	}

	public bool Has<T>()
	{
		return _registry.ContainsKey(typeof(T));
	}

	public void Del<T>()
	{
		Debug.Assert(Has<T>());
		var value = Get<T>();
		_registry.Remove(typeof(T));
		_delSignals.Notify(value);
	}

	public IDisposable OnDel<T>(Action<T> callback)
	{
		return _delSignals.Add(callback);
	}

	public IDisposable OnSet<T>(Action<T> callback)
	{
		return _setSignals.Add(callback);
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
