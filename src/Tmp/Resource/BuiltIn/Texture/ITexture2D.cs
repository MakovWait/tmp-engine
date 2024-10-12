using Raylib_cs;
using Tmp.Math;
using Tmp.Render;

namespace Tmp.Resource.BuiltIn.Texture;

public interface ITexture2D
{
    void Draw(IDrawContext ctx, Vector2 position, Color modulate);
    
    void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate);
    
    void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate);
}