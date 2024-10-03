using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render.Util;

namespace Tmp.Render;

// TODO weird args order
public class SubViewport(int screenHeight, int screenWidth, int virtualWidth, int virtualHeight, IDeferredValueMut<SubViewport.Texture> texture)
{
    private readonly SubViewports _subViewports = new();
    private readonly CanvasLayers _canvasLayers = new();
    private readonly Canvas _canvas = new();
    private readonly Camera2D _camera = new();

    private _RenderTexture2D _target;
    
    // TODO fix
    private float VirtualRatio => screenWidth / (float)virtualWidth;
    private int ScreenWidth => screenWidth;
    private int ScreenHeight => screenHeight;
    
    public void Load()
    {
        _target = Raylib.LoadRenderTexture(virtualWidth, virtualHeight);
        texture.Set(new Texture(this));
    }

    public void Unload()
    {
        Raylib.UnloadRenderTexture(_target);
    }
    
    public void Draw()
    {
        _subViewports.Draw();
        
        Raylib.BeginTextureMode(_target);
        Raylib.ClearBackground(Color.Black);
        
        Raylib.BeginMode2D(_camera);
        _canvas.Draw();
        Raylib.EndMode2D();

        _canvasLayers.Draw();

        Raylib.EndTextureMode();
    }

    internal void CreateContext(Component.Self self)
    {
        self.CreateContext<ICanvasItemContainer>(_canvas);
        self.CreateContext<ICanvasLayerContainer>(_canvasLayers);
        self.CreateContext<ISubViewportContainer>(_subViewports);
        self.CreateContext<ICamera2D>(_camera);
    }

    public class Texture(SubViewport subViewport)
    {
        public void Draw(IDrawContext ctx)
        {
            var source = new Rect2(0, 0, -subViewport._target.Texture.Width, subViewport._target.Texture.Height);
            var dest = new Rect2(
                -subViewport.VirtualRatio, 
                -subViewport.VirtualRatio,
                subViewport.ScreenWidth + (subViewport.VirtualRatio * 2),
                subViewport.ScreenHeight + (subViewport.VirtualRatio * 2)
            );
            
            ctx.DrawTexture(subViewport._target.Texture, source, dest, Color.White);
        }
    }
}
