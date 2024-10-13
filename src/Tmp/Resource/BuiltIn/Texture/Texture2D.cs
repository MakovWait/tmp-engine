using Raylib_cs;
using Tmp.IO;
using Tmp.Math;
using Tmp.Render;
using Tmp.Resource.Format;
using Tmp.Resource.Util;

namespace Tmp.Resource.BuiltIn.Texture;

public sealed class Texture2D(FilePath texturePath) : IDisposable, ITexture2D
{
    private readonly LazyRaylibTexture _texture = new(texturePath);
    
    public void Dispose()
    {
        _texture.Dispose();
    }
    
    public void Draw(IDrawContext ctx, Vector2 position, Color modulate)
    {
        ctx.DrawTextureRect(
            _texture, 
            new Rect2(position, new Vector2(_texture.Value.Width, _texture.Value.Height)),
            modulate
        );
    }

    public void DrawTextureRect(IDrawContext ctx, Rect2 rect, Color modulate)
    {
        ctx.DrawTextureRect(
            _texture, 
            rect,
            modulate
        );
    }

    public void DrawTextureRectRegion(IDrawContext ctx, Rect2 rect, Rect2 srcRect, Color modulate)
    {
        ctx.DrawTextureRectRegion(
            _texture, 
            rect,
            srcRect,
            modulate
        );
    }

    public class Deserializer : IResourceDeserializer<Texture2D>
    {
        public Texture2D From(ISerializeInput input)
        {
            return new Texture2D(
                input.ReadString(nameof(texturePath))
            );
        }

        public bool MatchType(string type)
        {
            return type == nameof(Texture2D);
        }

        public Y Deserialize<Y>(ISerializeInput input, IResultMapper<Y> resultMapper)
        {
            return resultMapper.Map(From(input));
        }
    }
}

internal class LazyRaylibTexture(string texturePath)
{
    private _Texture2D? _texture;
    public _Texture2D Value => _texture ??= Raylib.LoadTexture(texturePath);

    public void Dispose()
    {
        if (_texture is not null)
        {
            Raylib.UnloadTexture(_texture.Value);
        }
    }
    
    public static implicit operator _Texture2D(LazyRaylibTexture self) => self.Value;
}