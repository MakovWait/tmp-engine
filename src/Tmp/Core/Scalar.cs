using System.Diagnostics;

namespace Tmp.Core;

public interface IScalar<out T>
{
    T Value { get; }
}

public static class ScalarExtensions
{
    public static bool HasValue<T>(this IScalar<T?> self) where T : struct
    {
        return self.Value.HasValue;
    }

    public static bool HasValue<T>(this IScalar<T?> self) where T : class
    {
        return self.Value != null;
    }

    public static T GetNotNull<T>(this IScalar<T?> self) where T : struct
    {
        var value = self.Value;
        Debug.Assert(value.HasValue);
        return value.Value;
    }

    public static T GetNotNull<T>(this IScalar<T?> self) where T : class
    {
        var value = self.Value;
        Debug.Assert(value != null);
        return value;
    }
}