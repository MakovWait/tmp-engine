using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render.Util;
using Tmp.Resource.BuiltIn;

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

    public class Texture(SubViewport subViewport) : ITexture2D
    {
        public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, new Rect2(position, ScreenSize), Source, modulate);
        }

        public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }

        public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate)
        {
            ctx.DrawTextureRectRegion(TargetTexture, rect, Source, modulate);
        }
        
        private Rect2 Source => new(0, 0, subViewport._target.Texture.Width, -subViewport._target.Texture.Height);
        private _Texture2D TargetTexture => subViewport._target.Texture;
        private Vector2 ScreenSize => new(subViewport.ScreenWidth, subViewport.ScreenHeight);
    }
}
