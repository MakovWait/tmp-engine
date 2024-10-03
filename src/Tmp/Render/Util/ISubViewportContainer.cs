namespace Tmp.Render.Util;

public interface ISubViewportContainer
{
    void Add(SubViewport viewport);
    
    void Remove(SubViewport viewport);
}