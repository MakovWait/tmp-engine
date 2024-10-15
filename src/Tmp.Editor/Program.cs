using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using Tmp;
using Tmp.Core;
using Tmp.Core.Plugins;
using Tmp.Core.Plugins.Sources;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Project;
using Tmp.Render;
using Tmp.Render.Components;
using Tmp.Resource.BuiltIn.Texture;
using Tmp.Resource.Components;
using Tmp.Window;
using Tmp.Window.Rl;

var app = new App(new PluginSourceSequence<App>([
    new PluginEditor(),
    Project.GetPlugins(),
]));
await app.Run();

internal class PluginEditor() : PluginWrap<App>(new PluginAnonymous<App>("editor")
{
    OnBuild = app =>
    {
        app.Shelf.Set<IWindows>(new WindowsImGui());

        app.PreStart += () =>
        {
            Raylib.SetConfigFlags(ConfigFlags.TopmostWindow | ConfigFlags.ResizableWindow);
            Raylib.InitWindow(
                800,
                450,
                "Game"
            );
            Raylib.SetTargetFPS(60);    
        };
        
        app.PreUpdate += () =>
        {
            rlImGui.Begin();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
        };
        app.OnUpdate += () =>
        {
            if (ImGui.Begin("ddd"))
            {
                ImGui.End();   
            }
        };
        app.PostUpdate += () =>
        {
            rlImGui.End();
            Raylib.EndDrawing();
        };
        
        app.OnStart += () =>
        {
            rlImGui.Setup(true);
        };

        app.OnClose += () =>
        {
            rlImGui.Shutdown();
        };
    }
});

public class WindowImGui : IWindow
{
    private readonly RenderTexture2D _renderTexture2D;
    
    public WindowImGui(WindowSettings settings)
    {
        var size = settings.Size ?? new Vector2I(800, 450);
        _renderTexture2D = Raylib.LoadRenderTexture(size.X, size.Y);
    }
    
    public Vector2I Size { get; private set; }

    public void BeginDraw()
    {
        if (ImGui.Begin("Simple Window"))
        {
            Size = new Vector2I(ImGui.GetWindowSize().X.ToInt(), ImGui.GetWindowSize().Y.ToInt());
            Raylib.BeginTextureMode(_renderTexture2D);
            Raylib.ClearBackground(Color.White);
        }
    }
    
    public void EndDraw()
    {
        Raylib.EndTextureMode();
        rlImGui.ImageRenderTextureFit(_renderTexture2D);
        ImGui.End();
    }

    public void Close()
    {
        Raylib.UnloadRenderTexture(_renderTexture2D);
    }
}

public class WindowsImGui : IWindows
{
    private IWindow? _window;
    public IWindow MainWindow => _window!;
    
    public void Close()
    {
        _window?.Close();
    }

    public IWindow Create(WindowSettings settings)
    {
        _window = new WindowImGui(settings);
        return _window;
    }
}