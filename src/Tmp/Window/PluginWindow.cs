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
        var windows = app.Val<IWindows>().GetOr(() => new WindowsRl());
        
        app.DecorateRootUp(new Component(self =>
        {
            windows.Start(settings);
            
            var window = windows.Main;
            var viewport = window.Viewport;
            
            viewport.BindTo(self);
 
            self.UseCleanup(() => windows.Close());
            
            // TODO 🥶
            self.UseEffect(() =>
            {
                viewport.Load();
                return () => viewport.Unload();
            }, []);
                
            self.On<Draw>(() => window.Draw());
        }));
    }
});