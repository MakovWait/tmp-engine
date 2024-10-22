using Tmp.Core.Resource;

namespace Tmp.Tests.Resources;

public class Tests
{
    private IResources _resources;
    
    [SetUp]
    public void Setup()
    {
        _resources = new Core.Resource.Resources();    
    }

    [Test]
    public void TestInsert()
    {
        _resources.Set(1);
        Assert.Multiple(() =>
        {
            Assert.That(_resources.Has<int>(), Is.True);
            Assert.That(_resources.Get<int>(), Is.EqualTo(1));
        });
    }
    
    [Test]
    public void TestRemove()
    {
        _resources.Set(1);
        _resources.Del<int>();
        Assert.Multiple(() =>
        {
            Assert.That(_resources.Has<int>(), Is.False);
        });
    }
    
    [Test]
    public void TestOnRemove()
    {
        var wasRemoved = false;
        _resources.OnDel((int what) =>
        {
            Assert.That(what, Is.EqualTo(1));
            wasRemoved = true;
        });
        _resources.Set(1);
        _resources.Del<int>();
        Assert.That(wasRemoved, Is.True);
    }
    
    [Test]
    public void TestOnInsert()
    {
        var wasInserted = false;
        _resources.OnSet((int what) =>
        {
            Assert.That(what, Is.EqualTo(1));
            wasInserted = true;
        });
        _resources.Set(1);
        Assert.That(wasInserted, Is.True);
    }
    
    [Test]
    public void TestOnInsertDispose()
    {
        var wasInserted = false;
        _resources.OnSet((int _) =>
        {
            wasInserted = true;
        }).Dispose();
        _resources.Set(1);
        Assert.That(wasInserted, Is.False);
    }
    
    [Test]
    public void TestOnRemoveDispose()
    {
        var wasRemoved = false;
        _resources.OnDel((int _) =>
        {
            wasRemoved = true;
        }).Dispose();
        _resources.Set(1);
        _resources.Del<int>();
        Assert.That(wasRemoved, Is.False);
    }
    
    [Test]
    public void TestEffectCleanupIsCalledOnReSet()
    {
        var res = _resources.Res<int>();
        res.Set(1);
        
        var calls = 0;
        res.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        res.Set(2);
        res.Set(3);
        
        Assert.That(calls, Is.EqualTo(2));
    }
    
    [Test]
    public void TestEffectCleanupIsCalledOnDel()
    {
        var res = _resources.Res<int>();
        res.Set(1);
        
        var calls = 0;
        res.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        res.Del();
        
        Assert.That(calls, Is.EqualTo(1));
    }
    
    [Test]
    public void TestEffectDelCleanupIsOneShot()
    {
        var res = _resources.Res<int>();
        res.Set(1);
        
        var calls = 0;
        res.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        res.Del();
        res.Set(1);
        
        Assert.That(calls, Is.EqualTo(1));
    }
}