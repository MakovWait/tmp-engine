using Tmp.Asset;
using Tmp.Asset.Format;
using Tmp.Asset.Format.Text;
using Tmp.Asset.Util;

namespace Tmp.Tests.Asset;

public class TestLoad
{
    [Test]
    public void TestLoadUntyped()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);
        
        var untyped = assets.Load("ass://untyped-asset/asset.gres");
        Assert.That(untyped is Ass<TestAsset>);
    }
    
    [Test]
    public void TestLoadTyped()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);
        
        var typed = assets.Load<TestAsset>("ass://untyped-asset/asset.gres");
        Assert.That(typed.Get(), Is.Not.EqualTo(null));
    }
    
    [Test]
    public void TestLoadSubtyping()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);

        var b = assets.Load<ITestAsset>("ass://untyped-asset/asset.gres");
        var a = assets.Load<TestAsset>("ass://untyped-asset/asset.gres");
        
        Assert.That(b == a);
    }
    
    [Test]
    public void TestLoadSubtypingAnotherLoadOrder()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);

        var a = assets.Load<TestAsset>("ass://untyped-asset/asset.gres");
        var b = assets.Load<ITestAsset>("ass://untyped-asset/asset.gres");
        
        Assert.That(b == a);
    }
    
    private interface ITestAsset
    {
        
    }
    
    private class TestAsset : ITestAsset
    {
        public class Deserializer : IAssetDeserializer<TestAsset>
        {
            public TestAsset From(ISerializeInput input)
            {
                return new TestAsset();
            }

            public bool MatchType(string type)
            {
                return type == nameof(TestAsset);
            }

            public Y Deserialize<Y>(ISerializeInput input, IResultMapper<Y> resultMapper)
            {
                return resultMapper.Map(From(input));
            }
        }    
    }
}