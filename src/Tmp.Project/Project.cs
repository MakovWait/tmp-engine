using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;
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
                    new Component()
                    {
                        Name = "BulletsContainer"
                    }
                },
                new CCamera2D()
                {
                    Width = 400,
                    Height = 225
                },
                new ComponentFunc(self =>
                {
                    var bullets = new ReactiveList<Bullet>();
                    
                    self.On<InputEventKey>(e =>
                    {
                        if (e.IsJustPressed() && e.KeyCode == KeyboardKey.Space)
                        {
                            bullets.Add(new Bullet(Guid.NewGuid().ToString())
                            {
                                Dir = Vector2.Right
                            });
                        }

                        if (e.IsJustPressed() && e.KeyCode == KeyboardKey.Q)
                        {
                            bullets.Clear();
                        }
                    });
                    
                    return new Portal("../../CCanvasLayer/BulletsContainer".AsNodePathLocator())
                    {
                        new For<Bullet>
                        {
                            In = bullets,
                            Render = (bullet, _) => new CBullet(bullet)
                        }
                    };
                })
            }
        };
    }
}

internal class Bullet(string key) : For<Bullet>.IItem
{
    public string Key { get; } = key;
    
    public required Vector2 Dir { get; init; }
    
    public Transform2D Transform { get; init; } = Transform2D.Identity;
}

internal class CBullet(Bullet bullet) : Component
{
    protected override Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D(bullet.Transform);
        var canvasItem = self.UseCanvasItem(transform);
        
        canvasItem.OnDraw(ctx =>
        {
            ctx.DrawRect(new Rect2I(0, 0, 16, 16), Color.Blue);
        });
        
        self.On<Update>(dt =>
        {
            transform.Position += bullet.Dir * dt * 10;
        });
        
        return [];
    }
}