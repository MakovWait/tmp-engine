using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;
using Tmp.Resource.BuiltIn.Texture;

namespace Tmp.Render.Components;

public class CSubViewport(CSubViewport.Props props) : Component(self =>
{
    var container = self.Use<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.Size
    );
    viewport.CreateContext(self);
    
    self.CreateContext(new СNode2DTransform
    {
        Local = Transform2D.Identity
    });
    
    self.UseEffect(() =>
    {
        viewport.Load();
        props.Texture.Set(viewport.Texture);
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
        
        public IDeferredValueMut<ITexture2D> Texture { get; init; }
    }
}