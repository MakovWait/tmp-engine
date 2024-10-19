using Raylib_cs;
using Tmp.Math;
using Tmp.Render;

namespace Tmp.Window.Rl;

public class WindowsRl : IWindows
{
    private WindowRl? _window;

    public IWindow Create(WindowSettings settings)
    {
        var size = settings.Size ?? new Vector2I(800, 450);
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(
            size.X,
            size.Y,
            settings.Title ?? "Game"
        );
        Raylib.SetTargetFPS(settings.TargetFps ?? 60);
        _window = new WindowRl(new SubViewport(size));
        return _window;
    }
}

public class WindowRl(SubViewport viewport) : IWindow
{
    public void Update()
    {
        Raylib.BeginDrawing();

        Raylib.ClearBackground(Color.White);

        var size = new Vector2I(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

        // viewport.Resize(size);
        viewport.Draw();

        var viewportRenderTarget = viewport.RenderTarget;
        var source = SourceOf(viewportRenderTarget);
        Raylib.DrawTexturePro(
            viewport.RenderTarget.Texture,
            source,
            new Rect2(0, 0, size),
            Vector2.Zero,
            0,
            Color.White
        );

        Raylib.EndDrawing();
    }

    private Rect2 SourceOf(_RenderTexture2D render) => new(0, 0, render.Texture.Width, -render.Texture.Height);

    public SubViewport Viewport => viewport;
}
