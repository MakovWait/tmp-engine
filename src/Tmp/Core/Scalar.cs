using System.Diagnostics;

namespace Tmp.Core;

public interface IScalar<out T>
{
    T Get();
}

public static class ScalarExtensions
{
    public static bool HasValue<T>(this IScalar<T?> self) where T : struct
    {
        return self.Get().HasValue;
    }

    public static bool HasValue<T>(this IScalar<T?> self) where T : class
    {
        return self.Get() != null;
    }

    public static T GetNotNull<T>(this IScalar<T?> self) where T : struct
    {
        var value = self.Get();
        Debug.Assert(value.HasValue);
        return value.Value;
    }

    public static T GetNotNull<T>(this IScalar<T?> self) where T : class
    {
        var value = self.Get();
        Debug.Assert(value != null);
        return value;
    }
}