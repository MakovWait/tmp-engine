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
	
	public static Y Map<T, Y>(this IShelf self, Func<T, Y> map, Y @default = default!)
	{
		return self.Val<T>().Map(map, @default);
	}
	
	public static T Get<T>(this IShelf self, T orElse = default!)
	{
		return self.Val<T>().Map<T>(x => x, orElse);
	}
	
	public static T Get<T>(this IShelf self, Func<T> orElse)
	{
		return self.Val<T>().Map<T>(x => x, orElse());
	}
}