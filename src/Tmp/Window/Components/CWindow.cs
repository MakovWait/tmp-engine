using Tmp.Core;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Window.Rl;

namespace Tmp.Window.Components;

public readonly struct WindowSettings
{
    public string? Title { get; init; }
    public Vector2I? Size { get; init; }
    public int? TargetFps { get; init; }
}

public class CWindow(WindowSettings settings) : Component(self =>
{
    var windows = self.Use<IWindows>();

    self.UseEffect(() =>
    {
        var input = new Input(true);
        windows.Get().Start(settings, input);
        
        self.RuntimeRef.DecorateDown(new _CWindowInner(windows.Get().Main, input)
        {
            Children = self.Children
        });

        return () => throw new NotSupportedException();
    }, [windows]);

    return [];
});

internal class _CWindowInner(IWindow window, Input input) : Component(self =>
{
    window.BindTo(self);

    self.CreateContext(input);
    
    self.On<Draw>(() => window.Draw());
});

public class CWindowsRl() : Component(self =>
{
    var windows = new WindowsRl();
    self.CreateContext<IWindows>(windows);
    
    self.UseCleanup(() => windows.Close());
});
