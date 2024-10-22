using Tmp.Core.Redot;

namespace Tmp.Asset.Components;

public static class Hooks
{
    public static IAsset<T> UseAsset<T>(this Component.Self self, AssetPath path)
    {
        var assets = self.UseSingleton<IAssets>();
        return assets.Load<T>(path);
    }
}