using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;

namespace Tmp.Render.Components;

public class CCanvasItem(CCanvasItem.Props props) : Component(self =>
{
    var transform = self.UseTransform2D(props.Transform2D);
    var canvasItem = self.UseCanvasItem(transform);
    canvasItem.OnDraw(ctx =>
    {
        props.OnDraw?.Invoke(ctx);
    });
})
{
    public readonly struct Props
    {
        public Props()
        {
        }

        public Transform2D? Transform2D { get; init; } = null;

        public Action<IDrawContext>? OnDraw { get; init; } = null;
    }
}