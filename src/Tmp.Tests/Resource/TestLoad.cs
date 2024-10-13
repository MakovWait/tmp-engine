using Tmp.Resource;
using Tmp.Resource.Format;
using Tmp.Resource.Format.Text;
using Tmp.Resource.Util;

namespace Tmp.Tests.Resource;

public class TestLoad
{
    [Test]
    public void TestLoadUntyped()
    {
        var resources = new Resources();

        var textLoader = new ResourceLoaderText();
        textLoader.AddDeserializer(new TestRes.Deserializer());
        resources.AddLoader(textLoader);
        
        var untyped = resources.Load("res://untyped-res/res.gres");
        Assert.That(untyped is Res<TestRes>);
    }
    
    [Test]
    public void TestLoadTyped()
    {
        var resources = new Resources();

        var textLoader = new ResourceLoaderText();
        textLoader.AddDeserializer(new TestRes.Deserializer());
        resources.AddLoader(textLoader);
        
        var typed = resources.Load<TestRes>("res://untyped-res/res.gres");
        Assert.That(typed.Get(), Is.Not.EqualTo(null));
    }
    
    [Test]
    public void TestLoadSubtyping()
    {
        var resources = new Resources();

        var textLoader = new ResourceLoaderText();
        textLoader.AddDeserializer(new TestRes.Deserializer());
        resources.AddLoader(textLoader);

        var b = resources.Load<ITestRes>("res://untyped-res/res.gres");
        var a = resources.Load<TestRes>("res://untyped-res/res.gres");
        
        Assert.That(b == a);
    }
    
    [Test]
    public void TestLoadSubtypingAnotherLoadOrder()
    {
        var resources = new Resources();

        var textLoader = new ResourceLoaderText();
        textLoader.AddDeserializer(new TestRes.Deserializer());
        resources.AddLoader(textLoader);

        var a = resources.Load<TestRes>("res://untyped-res/res.gres");
        var b = resources.Load<ITestRes>("res://untyped-res/res.gres");
        
        Assert.That(b == a);
    }
    
    private interface ITestRes
    {
        
    }
    
    private class TestRes : ITestRes
    {
        public class Deserializer : IResourceDeserializer<TestRes>
        {
            public TestRes From(ISerializeInput input)
            {
                return new TestRes();
            }

            public bool MatchType(string type)
            {
                return type == nameof(TestRes);
            }

            public Y Deserialize<Y>(ISerializeInput input, IResultMapper<Y> resultMapper)
            {
                return resultMapper.Map(From(input));
            }
        }    
    }
}