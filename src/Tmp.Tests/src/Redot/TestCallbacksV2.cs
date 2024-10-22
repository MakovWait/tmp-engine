using Tmp.Core.Redot;

namespace Tmp.Tests.Redot;

public class TestCallbacksV2
{
    [Test]
    public void Smoke()
    {
        var callbacks = new Node.Callbacks();
        
        List<int> calls = [];
        callbacks.Add<float>(dt =>
        {
            calls.Add(1);
        });
        callbacks.Add<float>(dt =>
        {    
            calls.Add(2);
        });
        
        callbacks.Call(1f);
        
        Assert.That(calls, Is.EquivalentTo(new[] { 1, 2 }));
    }
    
    private struct A : IA {}

    private interface IA {}
    
    [Test]
    public void TestSubtyping()
    {
        var callbacks = new Node.Callbacks();
        
        List<int> calls = [];
        callbacks.Add<IA>(a =>
        {
            calls.Add(1);
        });
        callbacks.Add<A>(a =>
        {    
            calls.Add(2);
        });
        
        callbacks.Call(new A());
        
        Assert.That(calls, Is.EquivalentTo(new[] { 1, 2 }));
    }
}