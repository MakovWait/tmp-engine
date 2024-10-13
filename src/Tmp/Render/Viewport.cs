using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render.Util;

namespace Tmp.Render;

public class Viewport : IViewport
{
    private readonly SubViewports _subViewports;
    private readonly Canvas _canvas = new();
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Camera2D _camera = new();
    
    public Vector2I Size => new(Raylib.GetScreenWidth(), Raylib.GetScreenHeight());

    public Viewport()
    {
        _subViewports = new SubViewports(this);
    }
    
    public void Draw()
    {
        _subViewports.Draw();

        Raylib.ClearBackground(Color.White);
        Raylib.BeginDrawing();

        Raylib.BeginMode2D(_camera);
        _canvas.Draw();
        Raylib.EndMode2D();

        _canvasLayers.Draw();

        Raylib.EndDrawing();
    }

    internal void CreateContext(Component.Self self)
    {
        self.CreateContext<ICanvasItemContainer>(_canvas);
        self.CreateContext<ICanvasLayerContainer>(_canvasLayers);
        self.CreateContext<ISubViewportContainer>(_subViewports);
        self.CreateContext<ICamera2D>(_camera);
        self.CreateContext<IViewport>(this);
    }
}