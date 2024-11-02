using Raylib_cs;
using Tmp.Core;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render;
using Tmp.Window.Components;

namespace Tmp.Window.Rl;

public class WindowsRl : IWindows
{
    private WindowRl? _window;
    public IWindow Main => _window!;

    public void Start(WindowSettings settings, Input input)
    {
        var size = settings.Size ?? new Vector2I(800, 450);
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(
            size.X,
            size.Y,
            settings.Title ?? "Game"
        );
        Raylib.SetTargetFPS(settings.TargetFps ?? 60);
        _window = new WindowRl(
            new AppViewport(new SubViewport(size), input)
        );
    }

    public void Close()
    {
        Raylib.CloseWindow();
    }
}

public class WindowRl(AppViewport viewport) : IWindow, IAppViewportTarget
{
    public void Draw()
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);

        var size = new Vector2I(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());
        viewport.Draw(size, this);

        Raylib.EndDrawing();
    }

    public void BindTo(Component.Self self)
    {
        viewport.BindTo(self);
    }

    void IAppViewportTarget.Draw(_Texture2D texture, Rect2 rect, Rect2 sourceRect)
    {
        Raylib.DrawTexturePro(
            texture,
            sourceRect,
            rect,
            Vector2.Zero,
            0,
            Color.White
        );
    }
}
