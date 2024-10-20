using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Render;

namespace Tmp.Window;

public class AppViewport(SubViewport viewport)
{
    public void Load() => viewport.Load();

    public void Unload() => viewport.Unload();

    public void Draw(Vector2I windowSize, IAppViewportTarget target)
    {
        // viewport.Resize(windowSize);
        viewport.Draw();
        var texture = viewport.RenderTarget.Texture;
        // target.Draw(
        //     texture,
        //     new Rect2(0, 0, windowSize),
        //     new Rect2(0, 0, texture.Width, -texture.Height)
        // );

        var num = windowSize.X / (float)texture.Width;
        if ((double)texture.Height * num > windowSize.Y)
            num = windowSize.Y / (float)texture.Height;
        var destWidth = (int)(texture.Width * num);
        var destHeight = (int)(texture.Height * (double)num);
        var x = windowSize.X / 2f - destWidth / 2f;
        var y = windowSize.Y / 2f - destHeight / 2f;
        target.Draw(
            texture,
            new Rect2(x, y, destWidth, destHeight),
            new Rect2(0, 0, texture.Width, -texture.Height)
        );
    }

    internal void BindTo(Component.Self self)
    {
        viewport.BindTo(self);
    }
}

public interface IAppViewportTarget
{
    void Draw(_Texture2D texture, Rect2 rect, Rect2 sourceRect);
}