using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Redot;
using Tmp.HotReload.Components;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render;
using Tmp.Render.Components;

namespace Tmp.Project;

public static class Project
{
    public static void Init(Game game)
    {
        game.Init(tree =>
        {
            tree.DecorateRootUp(new CViewport());
            tree.DecorateRootUp(new CNode2DTransformRoot());
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
                                var canvasItem = self.UseCanvasItem(transform);

                                self.UseEffect(() =>
                                {
                                    transform.Get().Skew = -56f;
                                    // transform.Get().Scale = new Vector2(2, 2);
                                    // transform.Get().Rotation = new Degrees(45);
                                    // transform.Get().Position = new Vector2(6, 6);
                                }, [transform]);

                                self.On<Update>(dt => { transform.Get().Rotation += 45.DegToRad() * dt; });

                                canvasItem.OnDraw(ctx =>
                                {
                                    // ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.Red);
                                    ctx.DrawRect(new Rect2I(new Vector2I(-6, -6), new Vector2I(12, 12)), Color.Blue);
                                    ctx.DrawLine(new Vector2(0, 0), new Vector2(0, 10), Color.Green);
                                });
                            });
                        }),
                        // new Component(self =>
                        // {
                        //     var canvasItem = self.UseCanvasItem(self.UseTransform2D());
                        //     canvasItem.OnDraw(ctx =>
                        //     {
                        //         ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.White);
                        //     });
                        // }),
                        {
                            new CCamera2D(80, 45, new())
                        },
                    },
                    new CCanvasItem(new()
                    {
                        Transform2D = Transform2D.Identity.TranslatedLocal(new Vector2I(400, 225)),

                        OnDraw = ctx => { subViewportTexture.Get().Draw(ctx); }
                    }),
                    new CCanvasItem(new()
                    {
                        Transform2D = new Transform2D(0, new Vector2(-400, -225)),
                        OnDraw = ctx => { ctx.DrawFps(); }
                    }),
                    // new Component(self =>
                    // {
                    //     var canvasItem = self.UseCanvasItem(self.UseTransform2D());
                    //     canvasItem.OnDraw(ctx =>
                    //     {
                    //         ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.White);
                    //     });
                    // }),
                    new Component(self =>
                    {
                        var transform = self.UseTransform2D();
                        var canvasItem = self.UseCanvasItem(transform);

                        canvasItem.OnDraw(_ =>
                        {
                            Raylib.DrawLine(0, -4500, 0, 450, Color.Green);
                            Raylib.DrawLine(-8000, 0, 8000, 0, Color.Green);
                        });
                    }),

                    new Component(self =>
                    {
                        var transform = self.UseTransform2D();
                        
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
                            var canvasItem = self.UseCanvasItem(transform);

                            self.UseEffect(() =>
                            {
                                // transform.Get().Rotation = new Degrees(45);
                                // transform.Get().Position += new Vector2(6, 6);
                            }, [transform]);

                            self.On<Update>(dt =>
                            {
                                transform.Get().Rotation += 45.DegToRad() * dt;
                            });

                            canvasItem.OnDraw(ctx =>
                            {
                                // ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.Red);
                                ctx.DrawRect(new Rect2I(new Vector2I(-6, -6), new Vector2I(12, 12)), Color.Blue);
                            });
                        });
                    }),
                    new CCamera2D(400, 225, new()),
                    new Component<TestProps>(self =>
                    {
                        self.On<HotReloadUpdateApplication>(_ =>
                        {
                            Console.WriteLine("Hot reload!");
                        });
                        
                        self.On<Update>(dt =>
                        {
                            Console.WriteLine(self.Props.Value);
                        });
                    })
                    {
                        Props = () => new TestProps(10)
                    }
                ];
            }));
        });
    }
}

public readonly record struct TestProps(int Value);