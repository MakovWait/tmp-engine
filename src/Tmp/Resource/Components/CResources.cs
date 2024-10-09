using Tmp.Core.Redot;
using Tmp.Resource.BuiltIn;

namespace Tmp.Resource.Components;

public sealed class CResources(IResources resources) : Component(self =>
{
    self.SetSingleton(resources);
    resources.AddLoader(new Texture2D.Loader());
    
    self.UseEffect(() => resources.Dispose, []);
});