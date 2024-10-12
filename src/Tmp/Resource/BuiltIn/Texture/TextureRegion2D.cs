using Raylib_cs;
using Tmp.Math;
using Tmp.Render;
using Tmp.Resource.Format;

namespace Tmp.Resource.BuiltIn.Texture;

public class TextureRegion2D(Res<ITexture2D> origin, Rect2 srcRect) : ITexture2D, ISerializable
{
    public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
    {
        origin.Get().DrawTextureRectRegion(ctx, new Rect2(position, srcRect.Size), srcRect, modulate);
    }

    public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
    {
        origin.Get().DrawTextureRectRegion(ctx, rect, srcRect, modulate);
    }

    public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRectCustom, Color modulate)
    {
        origin.Get().DrawTextureRectRegion(ctx, rect, srcRect.Intersection(srcRectCustom), modulate);
    }

    void ISerializable.WriteTo(ISerializeOutput output)
    {
        output.Write(nameof(origin), origin);
        output.Write(nameof(srcRect), srcRect);
    }

    public class Deserializer : IResourceDeserializer<TextureRegion2D>
    {
        public TextureRegion2D From(ISerializeInput input)
        {
            return new TextureRegion2D(
                input.ReadSubRes<ITexture2D>(nameof(origin)),
                input.ReadRect2(nameof(srcRect))
            );
        }

        public bool MatchType(string type)
        {
            return type == nameof(TextureRegion2D);
        }
    }
}
