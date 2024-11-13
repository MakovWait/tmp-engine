using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Components;
using Tmp.Window.Components;

namespace Tmp.Project;

public static class Project
{
    public static Component GetRoot()
    {
        return new CWindow(new WindowSettings
        {
            Title = "Hello world!",
            Size = new Vector2I(800, 450),
            TargetFps = 60
        })
        {
            new CNode2DTransformRoot()
            {
                new CCanvasItem()
                {
                    OnDraw = ctx => ctx.DrawFps(),
                },
                new CCanvasLayer()
                {
                    new CCanvasItem()
                    {
                        OnDraw = ctx => ctx.DrawFps(),
                    },
                },
                new CCamera2D()
                {
                    Width = 400,
                    Height = 225
                }
            }
        };
    }
}