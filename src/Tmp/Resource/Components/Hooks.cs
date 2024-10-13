using Tmp.Core.Redot;

namespace Tmp.Resource.Components;

public static class Hooks
{
    public static IRes<T> UseRes<T>(this Component.Self self, ResourcePath path)
    {
        var resources = self.UseSingleton<IResources>();
        return resources.Load<T>(path);
    }
}