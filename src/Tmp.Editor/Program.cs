using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using Tmp;
using Tmp.Core.Plugins;
using Tmp.Core.Plugins.Sources;
using Tmp.Core.Shelf;
using Tmp.Math;
using Tmp.Project;
using Tmp.Render;
using Tmp.Window;

var app = new App(new PluginSourceSequence<App>([
    new DebugEditor(),
    Project.GetPlugins(),
]));
await app.Run();

internal class DebugEditor() : PluginWrap<App>(new PluginAnonymous<App>("debug-editor")
{
    OnBuild = app =>
    {
        app.SetVal<IWindows>(new WindowsImGui());
    }
});

internal class WindowImGui(AppViewport viewport) : IWindow, IAppViewportTarget
{
    public AppViewport Viewport => viewport;

    public void Draw()
    {
        Raylib.BeginDrawing();
        rlImGui.Begin();
        Raylib.ClearBackground(Color.White);

        if (ImGui.Begin("Game"))
        {
            var region = ImGui.GetContentRegionAvail();
            viewport.Draw(
                new Vector2I(region.X.ToInt(), region.Y.ToInt()),
                this
            );
            ImGui.End();
        }

        rlImGui.End();
        Raylib.EndDrawing();
    }

    void IAppViewportTarget.Draw(Texture2D texture, Rect2 rect, Rect2 sourceRect)
    {
        var destWidth = rect.Size.X.ToInt();
        var destHeight = rect.Size.Y.ToInt();
        ImGui.SetCursorPosX(0.0f);
        ImGui.SetCursorPosX(rect.Position.X);
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + rect.Position.Y);
        rlImGui.ImageRect(texture, destWidth, destHeight, sourceRect);
    }
}

internal class WindowsImGui : IWindows
{
    private IWindow? _window;

    public IWindow Main => _window!;

    public void Start(WindowSettings settings)
    {
        var size = settings.Size ?? new Vector2I(800, 450);
        Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
        Raylib.InitWindow(
            size.X,
            size.Y,
            settings.Title ?? "Game"
        );
        Raylib.SetTargetFPS(settings.TargetFps ?? 60);
        _window = new WindowImGui(new AppViewport(new SubViewport(size)));
        rlImGui.Setup(false);
    }

    public void Close()
    {
        rlImGui.Shutdown();
        Raylib.CloseWindow();
    }
}