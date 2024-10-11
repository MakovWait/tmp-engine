using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CCamera2D(
    float width, 
    float height, 
    Transform2D? initialTransform = null
) : Component(self =>
{
    var transform = self.UseTransform2D(initialTransform);
    var camera = self.Use<ICamera2D>();
    
    self.On<PreDraw>(() =>
    {
        camera.Get().Target = transform.Get().GlobalPosition;
        camera.Get().Offset = new Vector2(width, height);
    });
});