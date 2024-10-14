using Tmp.Core.Shelf;

namespace Tmp.Tests.Shelf;

public class Tests
{
    private IShelf _shelf;
    
    [SetUp]
    public void Setup()
    {
        _shelf = new Core.Shelf.Shelf();    
    }

    [Test]
    public void TestInsert()
    {
        _shelf.Set(1);
        Assert.Multiple(() =>
        {
            Assert.That(_shelf.Has<int>(), Is.True);
            Assert.That(_shelf.Get<int>(), Is.EqualTo(1));
        });
    }
    
    [Test]
    public void TestRemove()
    {
        _shelf.Set(1);
        _shelf.Del<int>();
        Assert.Multiple(() =>
        {
            Assert.That(_shelf.Has<int>(), Is.False);
        });
    }
    
    [Test]
    public void TestOnRemove()
    {
        var wasRemoved = false;
        _shelf.OnDel((int what) =>
        {
            Assert.That(what, Is.EqualTo(1));
            wasRemoved = true;
        });
        _shelf.Set(1);
        _shelf.Del<int>();
        Assert.That(wasRemoved, Is.True);
    }
    
    [Test]
    public void TestOnInsert()
    {
        var wasInserted = false;
        _shelf.OnSet((int what) =>
        {
            Assert.That(what, Is.EqualTo(1));
            wasInserted = true;
        });
        _shelf.Set(1);
        Assert.That(wasInserted, Is.True);
    }
    
    [Test]
    public void TestOnInsertDispose()
    {
        var wasInserted = false;
        _shelf.OnSet((int _) =>
        {
            wasInserted = true;
        }).Dispose();
        _shelf.Set(1);
        Assert.That(wasInserted, Is.False);
    }
    
    [Test]
    public void TestOnRemoveDispose()
    {
        var wasRemoved = false;
        _shelf.OnDel((int _) =>
        {
            wasRemoved = true;
        }).Dispose();
        _shelf.Set(1);
        _shelf.Del<int>();
        Assert.That(wasRemoved, Is.False);
    }
    
    [Test]
    public void TestEffectCleanupIsCalledOnReSet()
    {
        var val = _shelf.Val<int>();
        val.Set(1);
        
        var calls = 0;
        val.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        val.Set(2);
        val.Set(3);
        
        Assert.That(calls, Is.EqualTo(2));
    }
    
    [Test]
    public void TestEffectCleanupIsCalledOnDel()
    {
        var val = _shelf.Val<int>();
        val.Set(1);
        
        var calls = 0;
        val.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        val.Del();
        
        Assert.That(calls, Is.EqualTo(1));
    }
    
    [Test]
    public void TestEffectDelCleanupIsOneShot()
    {
        var val = _shelf.Val<int>();
        val.Set(1);
        
        var calls = 0;
        val.Effect(_ =>
        {
            return () => calls += 1;
        });
        
        val.Del();
        val.Set(1);
        
        Assert.That(calls, Is.EqualTo(1));
    }
}