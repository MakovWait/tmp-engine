using Tmp.Core.Redot;
using Tmp.Resource.BuiltIn.Texture;
using Tmp.Resource.Format;
using Tmp.Resource.Format.Text;

namespace Tmp.Resource.Components;

public sealed class CResources(IResources resources) : Component(self =>
{
    self.SetSingleton(resources);
    
    var textResLoader = new ResourceLoaderText();
    textResLoader.AddDeserializer(new TextureRegion2D.Deserializer());
    textResLoader.AddDeserializer(new Texture2D.Deserializer());
    
    resources.AddLoader(textResLoader);

    self.UseEffect(() => resources.Dispose, []);
});