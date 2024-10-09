using Raylib_cs;
using Tmp.Math;
using Tmp.Render;

namespace Tmp.Resource.BuiltIn;

public sealed class Texture2D(_Texture2D texture) : IDisposable, ITexture2D
{
    public void Dispose()
    {
        Raylib.UnloadTexture(texture);
    }
    
    public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
    {
        ctx.DrawTextureRect(
            texture, 
            new Rect2(position, new Vector2(texture.Width, texture.Height)),
            modulate
        );
    }

    public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
    {
        ctx.DrawTextureRect(
            texture, 
            rect,
            modulate
        );
    }

    public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate)
    {
        ctx.DrawTextureRectRegion(
            texture, 
            rect,
            srcRect,
            modulate
        );
    }

    public class Loader : IResourceLoader<Texture2D, NoSettings>
    {
        public Texture2D Load(ResourcePath path, NoSettings settings)
        {
            var texture = Raylib.LoadTexture(path.FilePath);
            return new Texture2D(texture);
        }

        public Texture2D Load(ResourcePath path)
        {
            return Load(path, new NoSettings());
        }
    
        public bool MatchPath(ResourcePath path)
        {
            return path.Extension == "png";
        }
    }
}

public interface ITexture2D
{
    void Draw(IDrawContext ctx, Vector2 position, Color modulate);
    
    void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate);
    
    void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate);
}