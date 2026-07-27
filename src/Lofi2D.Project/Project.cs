using Lofi2D.Asset;
using Lofi2D.Asset.Components;
using Lofi2D.Audio;
using Lofi2D.Audio.Components;
using Lofi2D.Core;
using Lofi2D.Core.Comp;
using Lofi2D.GUI;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render;
using Lofi2D.Render.Components;
using Lofi2D.Render.Texture;
using Lofi2D.Time;
using Lofi2D.Window.Components;
using Raylib_cs;

namespace Lofi2D.Project;

public static class Palette
{
    public static Color Background { get; } = new("23222a");
    public static Color Shadow { get; } = Colors.Black;
}

public static class Project
{
    public static Component GetRoot()
    {
        return new CFunc(root =>
        {
            var gameSize = new Vector2I(320, 180);
            var settingDrawGizmo = root.CreateContext(new SettingDrawGizmo(false));

            var boundsSize = new Vector2(320, 180);
            var bounds = root.CreateContext(new Bounds(new Rect2(-boundsSize / 2, boundsSize)));
            var snake = root.CreateContext(new Snake());
            root.CreateContext(new Food(bounds, snake));

            var viewportTexture = new Out<ITexture2D?>();

            return new CWindow(new WindowSettings
            {
                Title = "Hello world!",
                Size = new Vector2I(1280, 720),
                TargetFps = 60,
                ViewportSettings = new SubViewport.Settings
                {
                    Size = gameSize,
                    ClearColor = Palette.Background
                }
            })
            {
                new CAudioBusLayout(
                    new MasterBus
                    {
                        new AudioBus("Sfx"),
                        new AudioBus("Music"),
                    }
                )
                {
                    new CTime
                    {
                        new CAssets(new Assets())
                        {
                            new CNode2DTransformRoot()
                            {
                                new CSubViewport(new CSubViewport.Props
                                {
                                    Settings = new SubViewport.Settings
                                    {
                                        ClearColor = Colors.Transparent,
                                        Size = gameSize,
                                    },
                                    Texture = viewportTexture
                                })
                                {
                                    new CBoundsGizmo(bounds).If(settingDrawGizmo.Value),
                                    new CCamera2D()
                                    {
                                        Offset = gameSize / 2
                                    },
                                    new CSnake(),
                                    new CFood(),
                                    new CHud(),
                                    
                                    new CFunc(self =>
                                    {
                                        var parentControl = self.UseControl();
                                        parentControl.Size = new Vector2(32, 32);

                                        return [
                                            new CFunc(self =>
                                            {
                                                var control = self.UseControl();
                                                
                                                // control.MinSize = new Vector2(2, 2);
                                                control.Anchor = new AnchorPoints(1f, 0.0f, 1.0f, 0.0f);
                                                control.Offset = new AnchorOffsets(-10, 0, 0, 10);
                                                control.HGrow = Control.HGrowDirection.Both;
                                                control.VGrow = Control.VGrowDirection.Bottom;
                                                
                                                self.On<Update>(_ =>
                                                {
                                                    
                                                });
                                                
                                                return [];
                                            })
                                        ];
                                    
                                        return [];
                                    })
                                },

                                // shadow
                                new CFunc(self =>
                                {
                                    var transform = self.UseTransform2D();
                                    transform.Position += new Vector2(3, 3);

                                    var canvasItem = self.UseCanvasItem(transform);
                                    // canvasItem.Material = self.UseShaderMaterial(
                                    //     "assets://shaders/shadow_only/shader.jass",
                                    //     new IShaderMaterialParameters.Lambda(shader =>
                                    //     {
                                    //         shader.SetUniform("shadowColor", Palette.Shadow);
                                    //     })
                                    // );

                                    canvasItem.OnDraw(ctx =>
                                    {
                                        viewportTexture.Value!.Draw(ctx, Vector2.Zero, Palette.Shadow);
                                    });

                                    return [];
                                }),
                                new CFunc(self =>
                                {
                                    var transform = self.UseTransform2D();
                                    var canvasItem = self.UseCanvasItem(transform);

                                    canvasItem.OnDraw(ctx =>
                                    {
                                        viewportTexture.Value!.Draw(ctx, Vector2.Zero, Colors.White);
                                    });

                                    return [];
                                }),
                            }
                        }
                    }
                }
            };
        });
    }
}

public readonly record struct SettingDrawGizmo(bool Value);