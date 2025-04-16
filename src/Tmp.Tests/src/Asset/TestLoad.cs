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
        
        var untyped = assets.Load("assets://untyped-asset/asset.jass");
        Assert.That(untyped is _Asset<TestAsset>);
    }
    
    [Test]
    public void TestLoadTyped()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);
        
        var typed = assets.Load<TestAsset>("assets://untyped-asset/asset.jass");
        Assert.That(typed.Value, Is.Not.EqualTo(null));
    }
    
    [Test]
    public void TestLoadSubtyping()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);

        var b = assets.Load<ITestAsset>("assets://untyped-asset/asset.jass");
        var a = assets.Load<TestAsset>("assets://untyped-asset/asset.jass");
        
        Assert.That(b == a);
    }
    
    [Test]
    public void TestLoadSubtypingAnotherLoadOrder()
    {
        var assets = new Assets();

        var textLoader = new AssetLoaderText();
        textLoader.AddDeserializer(new TestAsset.Deserializer());
        assets.AddLoader(textLoader);

        var a = assets.Load<TestAsset>("assets://untyped-asset/asset.jass");
        var b = assets.Load<ITestAsset>("assets://untyped-asset/asset.jass");
        
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