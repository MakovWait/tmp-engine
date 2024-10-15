using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render.Util;

namespace Tmp.Render;

public interface IRenderTarget
{
    Vector2I Size { get; }
    
    void BeginDraw();

    void EndDraw();
}

public class Viewport : IViewport
{
    private readonly IRenderTarget _target;
    private readonly SubViewports _subViewports;
    private readonly Canvas _canvas = new();
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Camera2D _camera = new();
    
    public Vector2I Size => _target.Size;

    public Viewport(IRenderTarget target)
    {
        _target = target;
        _subViewports = new SubViewports(this);
    }
    
    public void Draw()
    {
        _subViewports.Draw();

        _target.BeginDraw();
        
        Raylib.BeginMode2D(_camera);
        _canvas.Draw();
        Raylib.EndMode2D();
        
        _canvasLayers.Draw();
        
        _target.EndDraw();
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