using Tmp.Core.Redot;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public static class Hooks
{
    public static CanvasItem UseCanvasItem(this Component.Self self, IDeferredValue<СNode2DTransform> transform)
    {
        var parent = self.Use<ICanvasItemContainer>();
        var item = new CanvasItem();

        self.CreateContext<ICanvasItemContainer>(item);

        self.UseEffect(() =>
        {
            parent.Get().AddChild(item);
            return () => parent.Get().RemoveChild(item);
        }, [parent]);

        self.On<PreDraw>(() =>
        {
            item.SetGlobalTransform(transform.Get().GlobalTransform);
        });

        return item;
    }
}