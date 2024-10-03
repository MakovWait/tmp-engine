using Tmp.Render.Util;

namespace Tmp.Render;

public class CanvasLayer : ICanvasItemContainer
{
    private readonly Canvas _canvas = new();

    public int Order { get; set; } = 0;
    
    public void Draw()
    {
        _canvas.Draw();
    }

    void ICanvasItemContainer.AddChild(CanvasItem child)
    {
        _canvas.AddChild(child);
    }

    void ICanvasItemContainer.RemoveChild(CanvasItem child)
    {
        _canvas.RemoveChild(child);
    }
}