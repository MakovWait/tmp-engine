using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CSubViewport(CSubViewport.Props props) : Component(self =>
{
    var container = self.Use<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.Size,
        props.Texture
    );
    viewport.CreateContext(self);
    
    self.CreateContext(new СNode2DTransform
    {
        Local = Transform2D.Identity
    });
    
    self.UseEffect(() =>
    {
        viewport.Load();
        return () => viewport.Unload();
    }, []);
    
    self.UseEffect(() =>
    {
        viewport.AddTo(container.Get());
        return () => viewport.RemoveFrom(container.Get());
    }, [container]);
})
{
    public readonly struct Props
    {
        public Vector2I Size { get; init; }
        
        public IDeferredValueMut<SubViewport.Texture> Texture { get; init; }
    }
}