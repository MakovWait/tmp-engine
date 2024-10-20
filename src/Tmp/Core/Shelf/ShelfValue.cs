using System.Diagnostics;

namespace Tmp.Core.Shelf;

public delegate Action Effect<in T>(T what);

public readonly struct ShelfValue<T>(IShelf shelf)
{
    public bool IsInserted() => shelf.Has<T>();

    public T Get()
    {
        Debug.Assert(shelf.Has<T>());
        return shelf.Get<T>();
    }
    
    public T GetOr(T orElse = default!)
    {
        return IsInserted() ? Get() : orElse;
    }
	
    public T GetOr(Func<T> orElse)
    {
        return IsInserted() ? Get() : orElse();
    }

    public void Set(T value)
    {
        shelf.Set(value);
    }

    public void Del()
    {
        shelf.Del<T>();
    }

    public bool Is(T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        return IsInserted() && value.Equals(Get());
    }

    public IDisposable OnDelete(Action<T> callback)
    {
        return shelf.OnDel(callback);
    }

    public IDisposable OnSet(Action<T> callback)
    {
        return shelf.OnSet(callback);
    }

    public IDisposable Effect(Effect<T> effect)
    {
        List<IDisposable> disposables = [];
        Action? cleanup = null;

        disposables.Add(shelf.OnSet<T>(what =>
        {
            cleanup?.Invoke();
            cleanup = effect(what);
        }));

        disposables.Add(shelf.OnDel<T>(_ =>
        {
            cleanup?.Invoke();
            cleanup = null;
        }));

        if (IsInserted())
        {
            cleanup = effect(Get());
        }

        return new Disposables(disposables);
    }

    public IDisposable Effect(Action<T> effect)
    {
        return Effect(what =>
        {
            effect(what);
            return () => { };
        });
    }

    public Y Map<Y>(Func<T, Y> map, Y @default = default!)
    {
        return IsInserted() ? map(Get()) : @default;
    }

    public Y Map<Y>(Func<T, Y> map, Func<Y> @default)
    {
        return IsInserted() ? map(Get()) : @default();
    }

    public bool Inspect(Action<T> inspect)
    {
        if (IsInserted())
        {
            inspect(Get());
            return true;
        }
        else
        {
            return false;
        }
    }

    private sealed class Disposables(List<IDisposable> disposables) : IDisposable
    {
        public void Dispose()
        {
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }

    public static implicit operator T(ShelfValue<T> self) => self.Get();
}