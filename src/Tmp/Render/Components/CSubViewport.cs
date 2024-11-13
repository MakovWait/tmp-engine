using Tmp.Asset.BuiltIn.Texture;
using Tmp.Core;
using Tmp.Core.Comp;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CSubViewport(CSubViewport.Props props) : ComponentFunc((self, children) =>
{
    var container = self.UseContext<ISubViewportContainer>();
    var viewport = new SubViewport(
        props.Size
    );
    viewport.BindTo(self);
    
    self.CreateContext(new СNode2DTransform
    {
        Local = Transform2D.Identity
    });

    props.Texture.Set(viewport.Texture);
    
    viewport.AddTo(container);
    self.OnCleanup(() => viewport.RemoveFrom(container));

    return children;
})
{
    public readonly struct Props
    {
        public Vector2I Size { get; init; }
        
        public IRefMut<ITexture2D?> Texture { get; init; }
    }
}