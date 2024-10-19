using Tmp.Core.Plugins;
using Tmp.Core.Redot;
using Tmp.Core.Shelf;
using Tmp.Math;
using Tmp.Window.Rl;

namespace Tmp.Window;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
}

public class PluginWindow(
    WindowSettings settings
) : PluginWrap<App>(new PluginAnonymous<App>("raylib-window")
{
    OnFinish = app =>
    {
        var windows = app.Get<IWindows>(() => new WindowsRl());
        var window = windows.Create(settings);
        var viewport = window.Viewport;
        
        app.Inspect<Tree>(tree =>
        {
            tree.OnInit += _ =>
            {
                tree.DecorateRootUp(new Component(self =>
                {
                    viewport.CreateContext(self);
                
                    self.UseEffect(() =>
                    {
                        viewport.Load();
                        return () => viewport.Unload();
                    }, []);
                
                    self.On<Draw>(() => window.Update());
                }));  
            };
        });
    }
});