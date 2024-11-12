using Tmp.Core.Comp;

namespace Tmp.Tests.ComponentsTree;

public class Tests
{
    [Test]
    public void Smoke()
    {
        var currentScope = new CurrentScope();
        var deferredQueue = new SignalBatch();
        var signals = new Signals(currentScope, deferredQueue);
        var tree = new Tree(currentScope, signals);
        var node = tree.CreateNode("test");

        node.Init(self =>
        {
            var signal = self.UseSignal(0).WithName("signal 1");
            var signal2 = self.UseSignal(2).WithName("signal 2");

            var mem = self.UseMemo(_ =>
            {
                self.SetScopeName("memo");
                Console.WriteLine($"memo {signal.Value} {signal2.Value}");
                return signal.Value + signal2.Value;
            }, 0).WithName("memo");

            Console.WriteLine($"now {mem.Value}");

            self.UseEffect(() =>
            {
                self.SetScopeName("effect");
                // self.UseEffect(() =>
                // {
                //     Console.WriteLine($"effect {signal.Value}");
                // });

                Console.WriteLine($"effect {mem.Value}");
            });

            // self.UseEffect(() =>
            // {
            //     var v = signal.Value;
            //     var v2 = signal2.Value;
            //     Console.WriteLine("changed");
            // });

            // self.UseEffect(() =>
            // {
            //     Console.WriteLine(signal.Value);
            //     
            //     self.UseEffect(() =>
            //     {
            //         self.Untrack(() =>
            //         {
            //             Console.WriteLine($"untracked scope {signal.Value}");
            //         });
            //         
            //         Console.WriteLine($"inner {signal2.Value}");
            //         
            //         self.OnCleanup(() =>
            //         {
            //             Console.WriteLine("inner Cleanup");
            //         });
            //     });
            //     
            //     self.OnCleanup(() =>
            //     {
            //         Console.WriteLine("outer Cleanup");
            //     });
            // });

            self.OnCleanup(() => { Console.WriteLine("node Cleanup"); });

            // signal.Value = 5;
            // Console.WriteLine("----");
            // signal2.Value = 6;
            // signal2.Value = 7;
            // signal2.Value = 8;

            // self.Batch(() =>
            // {
            //     self.Batch(() =>
            //     {
            //         signal.Value = 10;
            //     });
            //     signal2.Value = 10;
            // });

            // self.Batch(() =>
            // {
            //     signal2.Value = 10;
            //     signal.Value = 11;
            // });

            signal2.Value = 10;
            signal.Value = 11;
        });
        node.Mount();
    }

    [Test]
    public void Components()
    {
        var tree = new Tree();
        ISignalMut<bool>? cond = null;
        
        tree.Build(new ComponentFunc(self =>
        {
            cond = self.UseSignal(false);

            // var left = new TestComponent("left");
            // var right = new TestComponent("right");
            // var comp = self.UseMemo<IComponent>(
            //     _ => cond.Value ? left : right,
            //     left
            // );
            
            return [
                new ComponentFunc(self => []),
                new ComponentFunc(self =>
                {
                    self.OnMount(() =>
                    {
                        Console.WriteLine("mount!");
                    });
                    
                    self.OnCleanup(() =>
                    {
                        Console.WriteLine("bye!");
                    });

                    return [];
                }).If(cond)
            ];
        }) {Name = "root"});

        cond!.Value = true;
        cond!.Value = false;
        cond!.Value = true;
    }
}

public class UseEffectTests
{
    [Test]
    // so it is not called without tree::FlushQueue
    public void UseEffectIsDeferred()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.UseEffect(() =>
            {
                Assert.Fail();
            });
            
            return [];
        }));
        
        Assert.Pass();
    }
    
    [Test]
    // so it is called first time after tree::FlushQueue
    public void UseEffectIsDeferred2()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.UseEffect(() =>
            {
                Assert.Pass();
            });
            
            return [];
        }));
        
        tree.FlushDeferredQueue();
        
        Assert.Fail();
    }
    
    [Test]
    // so it is not triggered without tree::FlushQueue
    public void UseEffectIsDeferred3()
    {
        var tree = new Tree();
        
        ISignalMut<int>? signal = null;
        tree.Build(new ComponentFunc(self =>
        {
            signal = self.UseSignal(0);
            
            self.UseEffect(call =>
            {
                signal.Track();
                if (call == 2)
                {
                    Assert.Fail();
                }
                return call + 1;
            }, 1);
            
            return [];
        }));
        
        tree.FlushDeferredQueue();
        signal!.Value = 2;
        Assert.Pass();
    }
    
    [Test]
    // so it is triggered after tree::FlushQueue
    public void UseEffectIsDeferred4()
    {
        var tree = new Tree();
        
        ISignalMut<int>? signal = null;
        tree.Build(new ComponentFunc(self =>
        {
            signal = self.UseSignal(0);
            
            self.UseEffect(call =>
            {
                signal.Track();
                if (call == 2)
                {
                    Assert.Pass();
                }
                return call + 1;
            }, 1);
            
            return [];
        }));
        
        tree.FlushDeferredQueue();
        signal!.Value = 2;
        tree.FlushDeferredQueue();
        Assert.Fail();
    }
}

public class UseComputedTests
{
    [Test]
    public void ComputedIsCalledAsSoonCreated()
    {
        var tree = new Tree();
        
        tree.Build(new ComponentFunc(self =>
        {
            self.UseComputed(() =>
            {
                Assert.Pass();
            });
            
            return [];
        }));
        
        Assert.Fail();
    }
    
    [Test]
    public void ComputedIsCalledAsSoonTriggered()
    {
        var tree = new Tree();
        
        ISignalMut<int>? signal = null;
        tree.Build(new ComponentFunc(self =>
        {
            signal = self.UseSignal(0);
            
            self.UseComputed(calls =>
            {
                signal.Track();

                if (calls == 2)
                {
                    Assert.Pass();
                }
                
                return calls + 1;
            }, 1);
            
            return [];
        }));
        
        signal!.Value = 2;
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

public class UseMemoTests
{
    [Test]
    public void Smoke()
    {
        var tree = new Tree();
        ISignalMut<int>? counter = null;
        var useMemoCalls = 0;
        var useEffectCalls = 0;
        
        tree.Build(new ComponentFunc(self =>
        {
            counter = self.UseSignal(0);
            var doubleCounter = self.UseMemo(
                _ =>
                {
                    useMemoCalls++;
                    return counter.Value * 2;
                }, 
                counter.Value
            );
            
            self.UseEffect(call =>
            {
                doubleCounter.Track();
                doubleCounter.Track();
                doubleCounter.Track();
                var doubleValue = doubleCounter.Value; 
                if (call == 1)
                {
                    Assert.That(doubleValue, Is.EqualTo(0));
                }
                if (call == 2)
                {
                    Assert.That(doubleValue, Is.EqualTo(2));
                }
                useEffectCalls = call;
                return call + 1;
            }, 1);
            
            return [];
        }));

        Assert.That(useMemoCalls, Is.EqualTo(1));
        counter!.Value = 1;
        tree.FlushDeferredQueue();
        Assert.That(useMemoCalls, Is.EqualTo(2));
        Assert.That(useEffectCalls, Is.EqualTo(2));
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
        Assert.That(a.Name.Value, Is.EqualTo("a"));
        a.SetName("b");
        Assert.That(a.Name.Value, Is.EqualTo("@b@0"));
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
            Assert.That(a.Name.Value, Is.EqualTo("a"));
            Assert.That(a2.Name.Value, Is.EqualTo("@a@1"));
        });
    }
    
    [Test]
    public void TestNodeNameReactivity()
    {
        var tree = new Tree();
        Action? rename = null;
        
        tree.Build(new Component
        {
            Name = "root",
            Children = [
                new ComponentFunc(self =>
                {
                    rename = () => self.SetName("test");
                    
                    self.UseEffect(call =>
                    {
                        var name = self.Name.Value;
                        if (call == 2)
                        {
                            Assert.That(name, Is.EqualTo("test"));
                            Assert.Pass(); 
                        }
                        return call + 1;
                    }, 1);
                    
                    return [];
                })
                {
                    Name = "a",
                },
            ]
        });
        
        rename?.Invoke();
        tree.FlushDeferredQueue();
        Assert.Fail();
    }
    
    [Test]
    public void TestNodeRenameDoesNotTriggerUseEffectTwice()
    {
        var tree = new Tree();
        Action? rename = null;

        var totalCalls = 0;
        
        tree.Build(new Component
        {
            Name = "root",
            Children = [
                new ComponentFunc(self =>
                {
                    rename = () => self.SetName("b");
                    
                    self.UseEffect(() =>
                    {
                        self.Name.Track();
                        totalCalls += 1;
                    });
                    
                    return [];
                })
                {
                    Name = "a",
                },
                new Component()
                {
                    Name = "b",
                }
            ]
        });
        
        rename?.Invoke();
        tree.FlushDeferredQueue();
        Assert.That(totalCalls, Is.EqualTo(2));
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
            tree.Root.GetNode(new NodePath("/root"))!.Name.Value,
            Is.EqualTo("root")
        );
        
        Assert.That(
            tree.Root.GetNode(new NodePath("/root/a"))!.Name.Value,
            Is.EqualTo("a")
        );

        var a = tree.Root.GetNode(new NodePath("/root/a"))!;
        Assert.That(
            a.GetNode(new NodePath(".."))!.Name.Value,
            Is.EqualTo("root")
        );
        Assert.That(
            a.GetNode(new NodePath("../b"))!.Name.Value,
            Is.EqualTo("b")
        );
        Assert.That(
            a.GetNode(new NodePath("."))!.Name.Value,
            Is.EqualTo("a")
        );
        Assert.That(
            a.GetNode(new NodePath("./c"))!.Name.Value,
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