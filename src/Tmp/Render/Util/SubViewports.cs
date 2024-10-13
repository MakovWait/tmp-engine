namespace Tmp.Render.Util;

public class SubViewports(IViewport parent) : ISubViewportContainer
{
    private readonly List<ISubViewportContainer.IItem> _subViewports = [];
    
    public void Draw()
    {
        foreach (var subViewport in _subViewports)
        {
            subViewport.Draw();   
        }
    }

    public void Add(ISubViewportContainer.IItem viewport)
    {
        _subViewports.Add(viewport);
        viewport.AttachToParent(parent);
    }

    public void Remove(ISubViewportContainer.IItem viewport)
    {
        _subViewports.Remove(viewport);
        viewport.ClearParent();
    }
}