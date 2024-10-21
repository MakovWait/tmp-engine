using Tmp.Resource;

namespace Tmp.Tests.Resource;

public class TestDynRes
{
    [Test]
    public void TestAddDyn()
    {
        var resources = new Resources();

        var res = resources.AddDyn("test", "test");
        Assert.That(res.Get(), Is.EqualTo("test"));
    }

    [Test]
    public void TestDynLoad()
    {
        var resources = new Resources();
        resources.AddDyn("test", "test");

        var loadedRes = resources.Load<string>("dyn://test");
        Assert.That(loadedRes.Get(), Is.EqualTo("test"));
    }

    [Test]
    public void TestDynLoadIncorrectType()
    {
        var resources = new Resources();
        resources.AddDyn("test", "test");

        Assert.Throws<InvalidCastException>(() =>
        {
            resources.Load<int>("dyn://test");
        });
    }
    
    [Test]
    public void TestDynLoadTheOneThatHasNotBeenAdded()
    {
        var resources = new Resources();
        Assert.Throws<ArgumentException>(() =>
        {
            resources.Load<int>("dyn://test");
        });
    }
}