namespace Tmp.Render.Util;

public interface ISubViewportContainer
{
    void Add(IItem viewport);
    
    void Remove(IItem viewport);
    
    public interface IItem
    {
        public void AttachToParent(IViewport parent);

        public void ClearParent();

        public void Draw();
    }
}