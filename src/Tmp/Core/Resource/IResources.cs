namespace Tmp.Core.Resource;

public interface IResources
{
	void Set<T>(T value);

	T Get<T>();

	bool Has<T>();

	void Del<T>();

	IDisposable OnDel<T>(Action<T> callback);

	IDisposable OnSet<T>(Action<T> callback);
}

public static class ResourcesEx
{
	public static Res<T> Res<T>(this IResources self)
	{
		return new Res<T>(self);
	}
	
	public static Res<T> SetRes<T>(this IResources self, T value)
	{
		self.Set(value);
		return new Res<T>(self);
	}
}