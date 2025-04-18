using Tmp.Core.Comp;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public static class Hooks
{
    public static CanvasItem UseCanvasItem(this INodeInit self, СNode2DTransform transform)
    {
        var parent = self.UseContext<ICanvasItemContainer>();
        var item = new CanvasItem();

        self.CreateContext<ICanvasItemContainer>(item);

        parent.AddChild(item);
        self.OnLateCleanup(() =>
        {
            parent.RemoveChild(item);
        });

        self.On<PreDraw>(_ =>
        {
            item.SetFinalTransform(transform.Global);
        });

        return item;
    }
}