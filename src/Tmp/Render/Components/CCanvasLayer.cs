using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CCanvasLayer : Component
{
    protected override Core.Comp.Components Init(INodeInit self)
    {
        var layer = new CanvasLayer();

        self.CreateContext(new СNode2DTransform
        {
            Local = Transform2D.Identity
        });
        self.CreateContext<ICanvasItemContainer>(layer);

        var container = self.UseContext<ICanvasLayerContainer>();
        container.Add(layer);
        self.OnCleanup(() => container.Remove(layer));

        return Children;
    }
};