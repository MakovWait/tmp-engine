using Tmp.Asset.BuiltIn.Texture;
using Tmp.Asset.Format.Text;
using Tmp.Core.Redot;

namespace Tmp.Asset.Components;

public sealed class CAssets(IAssets assets) : Component(self =>
{
    self.SetSingleton(assets);
    
    var textLoader = new AssetLoaderText();
    textLoader.AddDeserializer(new TextureRegion2D.Deserializer());
    textLoader.AddDeserializer(new Texture2D.Deserializer());
    
    assets.AddLoader(textLoader);

    self.UseEffect(() => assets.Dispose, []);
});