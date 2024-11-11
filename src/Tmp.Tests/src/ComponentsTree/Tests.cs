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
        var node = tree.CreateNode();

        node.Init(self =>
        {
            var signal = self.CreateSignal(0).WithName("signal 1");
            var signal2 = self.CreateSignal(2).WithName("signal 2");

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
    }

    [Test]
    public void Components()
    {
        var tree = new Tree();
        ISignalMut<bool>? cond = null;
        
        tree.Build(new ComponentFunc(self =>
        {
            cond = self.CreateSignal(false);

            // var left = new TestComponent("left");
            // var right = new TestComponent("right");
            // var comp = self.UseMemo<IComponent>(
            //     _ => cond.Value ? left : right,
            //     left
            // );
            
            return [
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
        }));

        cond!.Value = true;
        cond!.Value = false;
        cond!.Value = true;
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