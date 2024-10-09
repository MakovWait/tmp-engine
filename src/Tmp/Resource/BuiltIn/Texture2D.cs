using Raylib_cs;
using Tmp.Math;
using Tmp.Render;

namespace Tmp.Resource.BuiltIn;

public sealed class Texture2D(_Texture2D texture) : IDisposable
{
    public void Dispose()
    {
        Raylib.UnloadTexture(texture);
    }
    
    public void Draw(IDrawContext ctx)
    {
        var rect = new Rect2(0, 0, texture.Width, texture.Height);
        ctx.DrawTexture(texture, rect, rect, Color.White);
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
