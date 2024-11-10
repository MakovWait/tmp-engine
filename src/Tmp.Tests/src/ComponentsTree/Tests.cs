using Tmp.Core.Components;

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

        node.Build(self =>
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
        Signal<bool>? signal = null;

        var component = new ComponentFunc(self =>
        {
            var cond = self.CreateSignal(false);
            signal = cond;

            self.UseEffect(() =>
            {
                Console.WriteLine("hello!");
            });

            self.OnCleanup(() =>
            {
                Console.WriteLine("root unmounted");
            });


            var left = new C2("left");
            var right = new C2("right");
            var comp = self.UseMemo<IComponent>(
                _ => cond.Value ? left : right,
                left
            );

            return
            [
                new C(comp),
                new ComponentFunc(self =>
                {
                    self.UseEffect(() =>
                    {
                        Console.WriteLine("child mounted");
                    });
                    self.OnCleanup(() =>
                    {
                        Console.WriteLine("child unmounted");
                    });
                    return [];
                }),
                new Conditional(cond)
                {
                    new ComponentFunc(self =>
                    {
                        self.SetScopeName("conditional");
                        self.OnMount(() => { Console.WriteLine("Hello!"); });
                        self.OnCleanup(() => { Console.WriteLine("Bye!"); });
                        return [];
                    })
                },
                
                new ComponentFunc(self =>
                {
                    self.SetScopeName("conditional2");
                    self.OnMount(() => { Console.WriteLine("Hello2!"); });
                    self.OnCleanup(() => { Console.WriteLine("Bye2!"); });
                    return [];
                }).If(cond)
            ];
        });
        
        tree.Build(component);
        Console.WriteLine("built");
        signal!.Value = true;
        // signal!.Value = true;
        // signal!.Value = true;
        signal!.Value = false;
        // signal!.Value = true;
    }
    
    private class C2(string text) : Component
    {
        protected override IEnumerable<IComponent> Build(Node self)
        {
            self.UseEffect(() => Console.WriteLine(text));
            return [];
        }
    }

    private class C(Signal<IComponent> comp) : Component
    {
        protected override IEnumerable<IComponent> Build(Node self)
        {
            self.UseEffect(() =>
            {
                var c = comp.Value;
                self.Untrack(() =>
                {
                    self.ReplaceChildren(c);
                });
            });
            
            return [];
        }
    }
}