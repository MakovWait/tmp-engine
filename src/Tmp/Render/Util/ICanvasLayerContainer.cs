namespace Tmp.Render.Util;

public interface ICanvasLayerContainer
{
    public void Add(CanvasLayer layer);

    public void Remove(CanvasLayer layer);
}