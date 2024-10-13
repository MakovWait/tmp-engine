using Tmp.Resource;

namespace Tmp.Tests.Resource;

public class TestResourcePath
{
    [Test]
    public void Test1()
    {
        var path = ResourcePath.FromPath("dyn://test");
        Assert.Multiple(() =>
        {
            Assert.That(path.IsDyn);
            Assert.That(!path.FilePath.Exists);
        });
    }

    [Test]
    public void TestResResourcePath()
    {
        var path = ResourcePath.FromPath("res://test.gres");
        Assert.Multiple(() =>
        {
            Assert.That(!path.IsDyn);
            Assert.That(path.FilePath.Exists);
        });
    }
    
    [Test]
    public void TestUserResourcePath()
    {
        var path = ResourcePath.FromPath("user://test.gres");
        Assert.That(!path.IsDyn);
    }
}