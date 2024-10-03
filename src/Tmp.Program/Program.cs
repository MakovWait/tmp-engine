using Raylib_cs;
using Tmp;
using Tmp.Core;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render;
using Tmp.Render.Components;

var game = new Game();

game.Tree.DecorateRootUp(new CViewport());
game.Tree.DecorateRootUp(new CNode2DTransformRoot());

game.Run(tree =>
{
    tree.AttachToRoot(new Component(self =>
    {
        var subViewportTexture = new DeferredValue<SubViewport.Texture>();

        return
        [
            new CSubViewport(new()
            {
                Texture = subViewportTexture,
                ScreenWidth = 800,
                ScreenHeight = 450,
                VirtualWidth = 160,
                VirtualHeight = 90
            })
            {
                new Component(self =>
                {
                    var transform = self.UseTransform2D();
                
                    self.On<Input<InputEventMouseMotion>>(e =>
                    {
                        Console.WriteLine(e);
                    });
                
                    self.On<Input<InputEventKey>>(e =>
                    {
                        Console.WriteLine(e);
                    });
                
                    self.On<Update>(dt =>
                    {
                        var dir = new Vector2();
                        if (Input.IsKeyPressed(KeyboardKey.D))
                        {
                            dir += Vector2.Right;
                        }
                
                        if (Input.IsKeyPressed(KeyboardKey.A))
                        {
                            dir += Vector2.Left;
                        }
                
                        if (Input.IsKeyPressed(KeyboardKey.W))
                        {
                            dir += Vector2.Up;
                        }
                
                        if (Input.IsKeyPressed(KeyboardKey.S))
                        {
                            dir += Vector2.Down;
                        }
                
                        dir = dir.Normalized();
                        transform.Get().Position += dir * 10 * dt;
                    });

                    return new Component(self =>
                    {
                        var transform = self.UseTransform2D();
                        
                        self.On<Update>(dt =>
                        {
                            transform.Get().Rotation += 45.ToRad() * dt;
                        });
                        
                        return new CCanvasItem(new()
                        {
                            Transform2D = Transform2D.Identity,
                            OnDraw = ctx =>
                            {
                                ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.Red);
                            }
                        });
                    });
                }),
                {
                    new CCamera2D(80, 45, new())
                },
            },
            new CCanvasItem(new()
            {
                Transform2D = Transform2D.Identity.TranslatedLocal(new Vector2I(400, 225)),
                
                OnDraw = ctx =>
                {
                    subViewportTexture.Get().Draw(ctx);
                }
            }),
            new CCanvasItem(new()
            {
                Transform2D = new Transform2D(0, new Vector2(-400, -225)),
                OnDraw = ctx =>
                {
                    ctx.DrawFps();
                }
            }),
            new CCamera2D(400, 225, new())
        ];
    }));
});
