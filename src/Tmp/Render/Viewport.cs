using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Render.Util;

namespace Tmp.Render;

public class Viewport
{
    private readonly SubViewports _subViewports = new();
    private readonly Canvas _canvas = new();
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Camera2D _camera = new();

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
    }
}