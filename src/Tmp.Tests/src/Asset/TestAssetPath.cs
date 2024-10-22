using Tmp.Asset;

namespace Tmp.Tests.Asset;

public class TestAssetPath
{
    [Test]
    public void Test1()
    {
        var path = AssetPath.FromPath("mem://test");
        Assert.Multiple(() =>
        {
            Assert.That(path.IsMem);
            Assert.That(!path.FilePath.Exists);
        });
    }

    [Test]
    public void TestResResourcePath()
    {
        var path = AssetPath.FromPath("assets://test.jass");
        Assert.Multiple(() =>
        {
            Assert.That(!path.IsMem);
            Assert.That(path.FilePath.Exists);
        });
    }
    
    [Test]
    public void TestUserResourcePath()
    {
        var path = AssetPath.FromPath("user://test.jass");
        Assert.That(!path.IsMem);
    }
}