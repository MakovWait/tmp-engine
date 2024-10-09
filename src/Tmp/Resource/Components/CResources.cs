using Tmp.Core.Redot;
using Tmp.Resource.BuiltIn;

namespace Tmp.Resource.Components;

public sealed class CResources(IResources resources) : Component(self =>
{
    // TODO introduce service/singleton
    self.CreateContext(resources);
    resources.AddLoader(new Texture2D.Loader());
    
    self.UseEffect(() => resources.Dispose, []);
});