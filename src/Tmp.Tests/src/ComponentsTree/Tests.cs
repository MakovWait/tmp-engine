using R3;
using Tmp.Core.Comp;
using Tmp.Core.Comp.Flow;

namespace Tmp.Tests.ComponentsTree;

public class Tests
{
    [Test]
    public void Smoke()
    {
        var tree = new Tree();
        tree.Build(new ComponentFunc(self =>
        {
            self.OnMount(() =>
            {
                Console.WriteLine("before children mount!!!");
                self.OnLateCleanup(() => Console.WriteLine("after children cleanup!!! 1"));
            });
            
            self.OnLateMount(() =>
            {
                Console.WriteLine("after children mount!!!");
                self.OnLateCleanup(() => Console.WriteLine("after children cleanup!!! 2"));
            });
            
            self.OnCleanup(() =>
            {
                Console.WriteLine("before children cleanup!!!");
            });
            
            self.CallDeferred(() =>
            {
                Console.WriteLine("deferred!!!");
            });
            
            return new ComponentFunc(self =>
            {
                self.OnMount(() =>
                {
                    Console.WriteLine("on child mount!!!");
                    self.OnLateCleanup(() => Console.WriteLine("on child cleanup!!!"));
                });
                
                return [];
            });
        }));
        tree.FlushDeferredQueue();
        tree.Free();
    }
}

public class ConditionalTests
{
    [Test]
    public void Smoke()
    {
        var cond = new ReactiveProperty<bool>(false);
        var tree = new Tree();
        tree.Build(new ComponentFunc(self =>
        {
            return new ComponentFunc(self =>
            {
                self.OnMount(() => Assert.Pass());
                return [];
            }).If(cond);
        }));
        cond.Value = true;
        tree.FlushDeferredQueue();
        Assert.Fail();
    }
    
    [Test]
    // TODO now it is not checked but should be one day
    public void UpdateIsQueued()
    {
        var cond = new ReactiveProperty<bool>(false);
        var tree = new Tree();
        tree.Build(new ComponentFunc(self =>
        {
            return new ComponentFunc(self =>
            {
                self.OnMount(() => Assert.Pass());
                return [];
            }).If(cond);
        }));
        cond.Value = true;
        cond.Value = false;
        cond.Value = true;
        tree.FlushDeferredQueue();
        Assert.Fail();
    }
}

public class ForTests
{
    [Test]
    // TODO now it is not checked but should be one day
    public void UpdateIsQueued()
    {
        var tree = new Tree();

        var items = new ReactiveList<Item>();
        tree.Build(new ComponentFunc(self =>
        {
            return new For<Item>
            {
                In = items,
                Render = (item, _) => new ComponentFunc(self =>
                {
                    self.OnMount(() => Console.WriteLine(item.Key) );
                    return [];
                })
            };
        }));
        
        items.Add(new Item("test"));
        items.Add(new Item("test2"));
        tree.FlushDeferredQueue();
        Assert.Pass();
    }
    
    [Test]
    public void Smoke()
    {
        var tree = new Tree();

        var items = new ReactiveList<Item>();
        tree.Build(new ComponentFunc(self =>
        {
            return new For<Item>
            {
                In = items,
                Render = (item, _) => new ComponentFunc(self =>
                {
                    self.OnMount(() =>
                    {
                        Assert.That(item.Key, Is.EqualTo("test"));
                        Assert.Pass();
                    });
                    return [];
                })
            };
        }));
        
        items.Add(new Item("test"));
        tree.FlushDeferredQueue();
        Assert.Fail();
    }

    [Test]
    public void ForIsDeferred()
    {
        var tree = new Tree();

        var items = new ReactiveList<Item>();
        tree.Build(new ComponentFunc(self =>
        {
            return new For<Item>
            {
                In = items,
                Render = (item, _) => new ComponentFunc(self =>
                {
                    self.OnMount(Assert.Pass);
                    return [];
                })
            };
        }));
        
        items.Add(new Item("test"));
        Assert.Fail();
    }
    
    private class Item(string key) : For<Item>.IItem
    {
        public string Key { get; } = key;
    }
}

public class PortalTests
{
    [Test]
    public void Smoke()
    {
        var tree = new Tree();

        tree.Build(new ComponentFunc(self =>
        {
            self.SetName("root");
            self.CreateContext(0);
            return
            [
                new ComponentFunc(self =>
                {
                    self.SetName("remote-parent");
                    self.CreateContext(1);
                    return [];
                }),
                new Portal("/root/remote-parent".AsNodePathLocator())
                {
                    new ComponentFunc(self =>
                    {
                        var ctx = self.UseContext<int>();

                        self.OnMount(() =>
                        {
                            Assert.That(ctx, Is.EqualTo(1));
                            Assert.Pass();
                        });

                        return [];
                    })
                }
            ];
        }));

        tree.FlushDeferredQueue();
        tree.FlushDeferredQueue();
        
        Assert.Fail();
    }
}

public class CallDeferredTests
{
    [Test]
    public void CallDeferredIsNotCalled()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.CallDeferred(_ =>
            {
                Assert.Fail();
            }, 0);
            
            return [];
        }));
        
        Assert.Pass();
    }
    
    [Test]
    public void CallDeferredIsCalled()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.CallDeferred(_ =>
            {
                Assert.Pass();
            }, 0);
            
            return [];
        }));
        
        tree.FlushDeferredQueue();
        Assert.Fail();
    }
}

public class NodeNameTests
{
    [Test]
    public void TestRenameDoesNotBreakChildrenNameUniqueness()
    {
        var tree = new Tree();
        
        tree.Build(new Component
        {
            Name = "root",
            Children = [
                new Component
                {
                    Name = "a",
                },
                new Component
                {
                    Name = "b",
                },
            ]
        });

        var a = tree.Root.GetNode(new NodePath("/root/a"))!;
        Assert.That(a.Name, Is.EqualTo("a"));
        a.SetName("b");
        Assert.That(a.Name, Is.EqualTo("@b@0"));
    }
    
    [Test]
    public void TestChildrenNameUniquenessInTreeBuild()
    {
        var tree = new Tree();
        
        tree.Build(new Component
        {
            Name = "root",
            Children = [
                new Component
                {
                    Name = "a",
                },
                new Component
                {
                    Name = "a",
                },
            ]
        });

        Assert.Multiple(() =>
        {
            Assert.That(tree.Root.GetNode(new NodePath("/root/a")), Is.Not.Null);
            Assert.That(tree.Root.GetNode(new NodePath("/root/@a@1")), Is.Not.Null);
        });
    }
    
    [Test]
    public void TestChildrenNameUniquenessInAddChild()
    {
        var tree = new Tree();

        var root = tree.CreateNode("root");
        var a = tree.CreateNode("a");
        var a2 = tree.CreateNode("a");
        
        root.AddChild(a);
        root.AddChild(a2);
        
        tree.Build(root);

        Assert.Multiple(() =>
        {
            Assert.That(a.Name, Is.EqualTo("a"));
            Assert.That(a2.Name, Is.EqualTo("@a@1"));
        });
    }
}

public class NodePathTests
{
    [Test]
    public void Smoke()
    {
        var tree = new Tree();
        
        tree.Build(new Component
        {
            Name = "root",
            Children = [
                new Component
                {
                    Name = "a",
                    Children = [
                        new Component
                        {
                            Name = "c"
                        }
                    ]
                },
                new Component
                {
                    Name = "b",
                },
            ]
        });
        
        Assert.That(
            tree.Root.GetNode(new NodePath("/root"))!.Name,
            Is.EqualTo("root")
        );
        
        Assert.That(
            tree.Root.GetNode(new NodePath("/root/a"))!.Name,
            Is.EqualTo("a")
        );

        var a = tree.Root.GetNode(new NodePath("/root/a"))!;
        Assert.That(
            a.GetNode(new NodePath(".."))!.Name,
            Is.EqualTo("root")
        );
        Assert.That(
            a.GetNode(new NodePath("../b"))!.Name,
            Is.EqualTo("b")
        );
        Assert.That(
            a.GetNode(new NodePath("."))!.Name,
            Is.EqualTo("a")
        );
        Assert.That(
            a.GetNode(new NodePath("./c"))!.Name,
            Is.EqualTo("c")
        );
    }
}

public class ContextTests
{
    [Test]
    public void OnlyParentContextIsUsed()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            var parentVal = self.CreateContext(1);
            return new ComponentFunc(self =>
            {
                var childVal = self.CreateContext(2);
                var usedVal = self.UseContext<int>();
                
                Assert.That(childVal, Is.Not.EqualTo(parentVal));
                Assert.That(usedVal, Is.EqualTo(parentVal));
                Assert.Pass();
                
                return [];
            });
        }));
        
        Assert.Fail();
    }
    
    [Test]
    public void CreateContextReturnsThePassedValue()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            var val = self.CreateContext(1);
            Assert.That(val, Is.EqualTo(1));
            Assert.Pass();

            return [];
        }));
        
        Assert.Fail();
    }
        
    [Test]
    public void UnableToUseCreatedContext()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.CreateContext(1);

            Assert.Throws<Exception>(() =>
            {
                self.UseContext<int>();
            });
            Assert.Pass();

            return [];
        }));
        
        Assert.Fail();
    }
}

public class CallbacksTests
{
    [Test]
    public void SmokeOn()
    {
        var tree = new Tree();
        tree.Build(new ComponentFunc(self =>
        {
            self.On<TestCallback>(args =>
            {
                Assert.That(args.Value, Is.EqualTo(10));
                Assert.Pass();
            });
            return [];
        }));
        
        tree.Call(new TestCallback(10));
        
        Assert.Fail();
    }
    
    [Test]
    public void SmokeLate()
    {
        var tree = new Tree();
        tree.Build(new ComponentFunc(self =>
        {
            self.OnLate<TestCallback>(args =>
            {
                Assert.That(args.Value, Is.EqualTo(10));
                Assert.Pass();
            });
            return [];
        }));
        
        tree.Call(new TestCallback(10));
        
        Assert.Fail();
    }
    
    [Test]
    public void CallbacksOrder()
    {
        var tree = new Tree();
        Calls calls = [];
        
        tree.Build(new ComponentFunc(self =>
        {
            self.On<TestCallback>(_ =>
            {
                calls.Log(1);
            });
            
            self.OnLate<TestCallback>(_ =>
            {
                calls.Log(4);
            });
            
            return new ComponentFunc(self =>
            {
                self.On<TestCallback>(_ =>
                {
                    calls.Log(2);
                });
            
                self.OnLate<TestCallback>(_ =>
                {
                    calls.Log(3);
                });

                return [];
            });
        }));
        
        tree.Call(new TestCallback(10));
        
        calls.AssertOrderIs([1, 2, 3, 4]);
    }

    private readonly record struct TestCallback(int Value);
}