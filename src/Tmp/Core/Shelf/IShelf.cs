namespace Tmp.Core.Shelf;

public interface IShelf
{
	void Set<T>(T value);

	T Get<T>();

	bool Has<T>();

	void Del<T>();

	IDisposable OnDel<T>(Action<T> callback);

	IDisposable OnSet<T>(Action<T> callback);
}

public static class ShelfEx
{
	public static ShelfValue<T> Val<T>(this IShelf self)
	{
		return new ShelfValue<T>(self);
	}
	
	public static ShelfValue<T> SetVal<T>(this IShelf self, T value)
	{
		self.Set(value);
		return new ShelfValue<T>();
	}
}