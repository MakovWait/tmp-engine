using Tmp.Asset;

namespace Tmp.Tests.Asset;

public class TestMemRes
{
    [Test]
    public void TestAddMem()
    {
        var assets = new Assets();

        var asset = assets.AddMem("test", "test");
        Assert.That(asset.Get(), Is.EqualTo("test"));
    }

    [Test]
    public void TestMemLoad()
    {
        var assets = new Assets();
        assets.AddMem("test", "test");

        var loadedAsset = assets.Load<string>("mem://test");
        Assert.That(loadedAsset.Get(), Is.EqualTo("test"));
    }

    [Test]
    public void TestMemLoadIncorrectType()
    {
        var assets = new Assets();
        assets.AddMem("test", "test");

        Assert.Throws<InvalidCastException>(() =>
        {
            assets.Load<int>("mem://test");
        });
    }
    
    [Test]
    public void TestMemLoadTheOneThatHasNotBeenAdded()
    {
        var assets = new Assets();
        Assert.Throws<ArgumentException>(() =>
        {
            assets.Load<int>("mem://test");
        });
    }
}