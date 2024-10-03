using Tmp.Core.Redot;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CSubViewport(CSubViewport.Props props) : Component(self =>
{
    var container = self.Use<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.ScreenHeight,
        props.ScreenWidth,
        props.VirtualWidth,
        props.VirtualHeight,
        props.Texture
    );
    viewport.CreateContext(self);
    
    self.UseEffect(() =>
    {
        viewport.Load();
        return () => viewport.Unload();
    }, []);
    
    self.UseEffect(() =>
    {
        container.Get().Add(viewport);
        return () => container.Get().Remove(viewport);
    }, [container]);
})
{
    public readonly struct Props
    {
        public Props()
        {
            
        }
        
        public int VirtualWidth { get; init; }
        
        public int VirtualHeight { get; init; }
        
        public int ScreenWidth { get; init; }
        
        public int ScreenHeight { get; init; }
        
        public IDeferredValueMut<SubViewport.Texture> Texture { get; init; }
    }
}