using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CCamera2D(float width, float height, CCamera2D.Props props) : Component(self =>
{
    var transform = self.UseTransform2D(props.Transform2D);
    var camera = self.Use<ICamera2D>();
    
    self.On<PreDraw>(() =>
    {
        camera.Get().Target = transform.Get().GlobalPosition;
        camera.Get().Offset = new Vector2(width, height);
    });
})
{
    public readonly struct Props
    {
        public Props()
        {
        }
        
        public Transform2D? Transform2D { get; init; } = null;
    }
}