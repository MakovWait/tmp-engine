using Tmp.Core.Redot;

namespace Tmp.Tests.Redot;

public class ContextTests
{
    [Test]
    public void TestContextSmoke()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.CreateContext(1);

            return new Component(compB =>
            {
                var i = compB.Use<int>();
                compB.UseEffect(() =>
                {
                    Assert.That(i.Get(), Is.EqualTo(1));
                    Assert.Pass(); 
                });
            });
        }));
        Assert.Fail();
    }
    
    [Test]
    public void TestNodeId()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.CreateContext(1);
            compA.SetId("compA");

            return new Component(compB =>
            {
                compB.UseEffect(() =>
                {
                    compB.GetNodeById("compA").CreateChild(new Component(newChild =>
                    {
                        newChild.UseEffect(() =>
                        {
                            Assert.Pass();
                        });
                    }));
                });
            });
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestContextInRuntime()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.CreateContext(1);

            return new Component(compB =>
            {
                compB.UseEffect(() =>
                {
                    compB.Unchecked.CreateChild(new Component(compC =>
                    {
                        var i = compC.Use<int>();
                        compC.UseEffect(() =>
                        {
                            Assert.That(i.Get(), Is.EqualTo(1));
                            Assert.Pass();     
                        });                        
                    }));
                });
            });
        }));
        Assert.Fail();
    }
    
    [Test]
    public void TestContextWithDeps()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            var ctx = compA.CreateContext(1);
            compA.On<TestCallback>(_ =>
            {
                ctx.Replace(2);
            });

            return new Component(compB =>
            {
                var i = compB.Use<int>();
                var calls = 0;
                compB.UseEffect(() =>
                {
                    calls += 1;
                    
                    switch (calls)
                    {
                        case 1:
                            Assert.That(i.Get(), Is.EqualTo(1));
                            break;
                        case 2:
                            Assert.That(i.Get(), Is.EqualTo(2));
                            break;
                    }

                    return () =>
                    {
                        switch (calls)
                        {
                            case 1:
                                Assert.That(i.Get(), Is.EqualTo(1));
                                break;
                            case 2:
                                Assert.That(i.Get(), Is.EqualTo(2));
                                Assert.Pass();
                                break;
                        }
                    };
                }, i);
            });
        }));
        tree.Call<TestCallback>();
        tree.Update();
        tree.Free();
        Assert.Fail();
    }
    
    [Test]
    public void TestContextOverride()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            var ctx = compA.CreateContext(1);
            compA.On<TestCallback>(() =>
            {
                ctx.Replace(2);
            });

            return new Component(compB =>
            {
                var i = compB.Use<int>();
                compB.UseEffect(() =>
                {
                    Assert.That(i.Get(), Is.EqualTo(1));
                });
                
                compB.On<TestCallback>(() =>
                {
                    Assert.That(i.Get(), Is.EqualTo(2));
                    Assert.Pass();
                });
            });
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.Fail();
    }
    
    
    [Test]
    public void TestAfterCallback()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.On<TestCallback>(() =>
            {
                calls.Add(1);
            });
            
            compA.After<TestCallback>(() =>
            {
                calls.Add(4);
            });

            return new Component(compB =>
            {
                compB.On<TestCallback>(() =>
                {
                    calls.Add(2);
                });
            
                compB.After<TestCallback>(() =>
                {
                    calls.Add(3);
                });
            });
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> {1, 2, 3, 4}));
    }
    
    [Test]
    public void TestContextOverrideDoesNotAffectNotRelated()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            var ctx = compA.CreateContext(1);
            compA.On<TestCallback>(() =>
            {
                ctx.Replace(3);
            });

            return new Component(compB =>
            {
                compB.CreateContext(2);
                
                return new Component(compC =>
                {
                    var i = compC.Use<int>();
                    compC.UseEffect(() =>
                    {
                        Assert.That(i.Get(), Is.EqualTo(2));
                    });
                    
                    compC.On<TestCallback>(() =>
                    {
                        Assert.That(i.Get(), Is.EqualTo(2));
                        Assert.Pass();
                    });
                });
            });
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestContextOnExitTree()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.CreateContext(1);
            compA.QueueFree();

            return new Component(compB =>
            {
                var i = compB.Use<int>();
                compB.UseEffect(() =>
                {
                    return () =>
                    {
                        Assert.That(i.Get(), Is.EqualTo(1));
                        Assert.Pass();
                    };
                });
            });
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestContextFindingStartsFromParent()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.CreateContext(1);

            return new Component(compB =>
            {
                compB.CreateContext(2);
                var i = compB.Use<int>();
                compB.UseEffect(() =>
                {
                    Assert.That(i.Get(), Is.EqualTo(1));
                    Assert.Pass();
                });
            });
        }));
        tree.Update();
        Assert.Fail();
    }
}

public class LifecycleEffectTests
{
    [Test]
    public void TestEffectEnterTree1()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(() =>
            {
                Assert.Pass();
                return () => { };
            });
        }));
        Assert.Fail();
    }
    
    [Test]
    public void TestEffectEnterTree2()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(Assert.Pass);
        }));
        Assert.Fail();
    }
    
    [Test]
    public void TestEffectEnterTreeEffectSequence()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(() => calls.Add(1));
            compA.UseEffect(() => calls.Add(2));
        }));
        Assert.That(calls, Is.EqualTo(new List<int> {1, 2}));
    }
    
    [Test]
    public void TestEffectExitTree()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(() => Assert.Pass);
            compA.QueueFree();
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestSelfDeathRattle()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(() => Assert.Pass, []);
            compA.QueueFree();
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestChildDeathRattle()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.QueueFree();

            return new Component(compB =>
            {
                compB.UseEffect(() => Assert.Pass, []);
            });
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestEffectExitTreeInChildWhenParentIsQueuedToFree()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.QueueFree();
            return new Component(compB =>
            {
                compB.UseEffect(Assert.Pass);
            });
        }));
        tree.Update();
        Assert.Fail();
    }
}

public class ComponentTests
{
    [Test]
    public void TestOuterChildren()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.On<TestCallback>(() => calls.Add(1));
            return [
                ..compA.Children,
                new Component(compB =>
                {
                    compB.On<TestCallback>(() => calls.Add(2));
                }),
            ];
        })
        {
            new Component(compC =>
            {
                compC.On<TestCallback>(() => calls.Add(3));
            })
        });
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> {1, 3, 2}));
    }
    
    [Test]
    public void TestOuterChildren2()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component
        {
            new Component(compC =>
            {
                compC.On<TestCallback>(() => calls.Add(3));
            })
        });
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> { 3 }));
    }
    
    [Test]
    public void TestOuterChildren3()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(_ => {})
        {
            new Component(compC =>
            {
                compC.On<TestCallback>(() => calls.Add(3));
            })
        });
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> { 3 }));
    }
}

public class CallbackTests
{
    [Test]
    public void TestCallback()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.On<TestCallback>(Assert.Pass);
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestMultipleCallbacks()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.On<TestCallback>(() => calls.Add(1));
            compA.On<TestCallback>(() => calls.Add(2));
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> {1, 2}));
    }
    
    [Test]
    public void TestCallbacksAreCalledInTree()
    {
        var tree = new Tree();
        var calls = new List<int>();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.On<TestCallback>(() => calls.Add(1));
            return new Component(compB =>
            {
                compB.On<TestCallback>(() => calls.Add(2));
            });
        }));
        tree.Call<TestCallback>();
        tree.Update();
        Assert.That(calls, Is.EqualTo(new List<int> {1, 2}));
    }
}

public readonly record struct TestCallback;