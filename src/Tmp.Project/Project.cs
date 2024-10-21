using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Plugins;
using Tmp.Core.Plugins.Sources;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Components;
using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Components;
using Tmp.Window;

namespace Tmp.Project;

public static class Project
{
    public static IPluginSource<App> GetPlugins()
    {
        return new PluginSourceSequence<App>([
            new PluginWindow(new WindowSettings
            {
                Title = "Hello world!",
                Size = new Vector2I(800, 450),
                TargetFps = 60
            }),
            new PluginTree(InitTree),
        ]);
    }

    private static void InitTree2(App tree)
    {
        // tree.DecorateRootUp(new CViewport());
        tree.DecorateRootUp(new CNode2DTransformRoot());
        tree.AttachToRoot(new CCanvasItem(new()
        {
            OnDraw = ctx =>
            {
                ctx.DrawFps();
            }
        })
        {
            new CCanvasLayer()
            {
                new CCanvasItem(new()
                {
                    OnDraw = ctx =>
                    {
                        ctx.DrawFps();
                    }
                })  
            },
            new CCamera2D(400, 225)
        });
    }
    
    private static void InitTree(App tree)
    {
        // tree.DecorateRootUp(new CViewport());
        tree.DecorateRootUp(new CNode2DTransformRoot());
        tree.AttachToRoot(new Component(self =>
        {
            var subViewportTexture = new DeferredValue<ITexture2D>();

            return
            [
                new CSubViewport(new()
                {
                    Texture = subViewportTexture,
                    // Size = new Vector2I(160, 90),
                    Size = new Vector2I(160, 90),
                })
                {
                    new Component(self =>
                    {
                        var transform = self.UseTransform2D();
                        var input = self.UseSingleton<Input>();

                        self.On<Update>(dt =>
                        {
                            var dir = new Vector2();
                            if (input.IsKeyPressed(KeyboardKey.D))
                            {
                                dir += Vector2.Right;
                            }

                            if (input.IsKeyPressed(KeyboardKey.A))
                            {
                                dir += Vector2.Left;
                            }

                            if (input.IsKeyPressed(KeyboardKey.W))
                            {
                                dir += Vector2.Up;
                            }

                            if (input.IsKeyPressed(KeyboardKey.S))
                            {
                                dir += Vector2.Down;
                            }

                            if (input.IsKeyJustPressed(KeyboardKey.Space))
                            {
                                self.RuntimeRef.CreateChild(new Component(self =>
                                {
                                    var transform = self.UseTransform2D();
                                    var canvasItem = self.UseCanvasItem(transform);

                                    self.On<Update>(dt => { transform.Get().Position += Vector2.Right * dt * 16; });

                                    canvasItem.OnDraw(ctx => { ctx.DrawRect(new Rect2I(-8, -8, 16, 16), Color.Red); });
                                }));
                            }

                            dir = dir.Normalized();
                            transform.Get().Position += dir * 10 * dt;
                        });

                        return new Component(self =>
                        {
                            var transform = self.UseTransform2D();
                            var canvasItem = self.UseCanvasItem(transform);

                            var texture = self.UseAss<ITexture2D>("ass://test.gres");

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
                                ctx.DrawRect(new Rect2I(new Vector2I(-6, -6), new Vector2I(12, 12)),
                                    Color.Blue);
                                ctx.DrawLine(new Vector2(0, 0), new Vector2(0, 10), Color.Green);

                                // var t = new TextureRegion2D(texture, new Rect2(0, 0, 32, 32));
                                // t.Draw(ctx, new Vector2(-16, -16), Color.White);
                                texture.Get().Draw(ctx, new Vector2(-16, -16), Color.White);
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
                        new CCamera2D(80, 45)
                    },
                },
                new CCanvasItem(new()
                {
                    Transform2D = new Transform2D(0, new Vector2(-400, -225)),

                    OnDraw = ctx => { subViewportTexture.Get().Draw(ctx, Vector2.Zero, Color.White); }
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
                    var input = self.UseSingleton<Input>();

                    self.On<Update>(dt =>
                    {
                        var dir = new Vector2();
                        if (input.IsKeyPressed(KeyboardKey.D))
                        {
                            dir += Vector2.Right;
                        }

                        if (input.IsKeyPressed(KeyboardKey.A))
                        {
                            dir += Vector2.Left;
                        }

                        if (input.IsKeyPressed(KeyboardKey.W))
                        {
                            dir += Vector2.Up;
                        }

                        if (input.IsKeyPressed(KeyboardKey.S))
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

                        self.On<Update>(dt => { transform.Get().Rotation += 45.DegToRad() * dt; });

                        canvasItem.OnDraw(ctx =>
                        {
                            // ctx.DrawRect(new Rect2I(Vector2I.Zero, new Vector2I(12, 12)), Color.Red);
                            ctx.DrawRect(new Rect2I(new Vector2I(-6, -6), new Vector2I(12, 12)),
                                Color.Blue);
                        });
                    });
                }),
                new CCamera2D(400, 225)
            ];
        }));
    }
}