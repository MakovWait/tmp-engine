using Tmp.Core.Redot;

namespace Tmp.Resource.Components;

public static class Hooks
{
    public static IDeferredValue<Res<T>> UseRes<T>(this Component.Self self, ResourcePath path)
    {
        var resources = self.Use<IResources>();
        var res = new DeferredValue<Res<T>>();
        self.UseEffect(() =>
        {
            res.Set(resources.Get().Load<T>(path));
        }, [resources]);
        return res;
    }
}