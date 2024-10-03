namespace Tmp.Render.Util;

public class SubViewports : ISubViewportContainer
{
    private readonly List<SubViewport> _subViewports = [];
    
    public void Draw()
    {
        foreach (var subViewport in _subViewports)
        {
            subViewport.Draw();   
        }
    }

    public void Add(SubViewport viewport)
    {
        _subViewports.Add(viewport);
    }

    public void Remove(SubViewport viewport)
    {
        _subViewports.Remove(viewport);
    }
}