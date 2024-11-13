using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CCamera2D : Component
{
    public float Width { get; init; }
    
    public float Height { get; init; }
    
    public Transform2D? InitialTransform { get; init; }

    protected override Core.Comp.Components Init(INodeInit self)
    {
         var transform = self.UseTransform2D(InitialTransform);
         var camera = self.UseContext<ICamera2D>();
         
         self.On<PreDraw>(_ =>
         {
             camera.Target = transform.GlobalPosition;
             camera.Offset = new Vector2(Width, Height);
         });
         
         return Children;
    }
}