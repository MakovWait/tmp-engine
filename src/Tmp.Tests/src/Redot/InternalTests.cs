using Tmp.Core.Redot;

namespace Tmp.Tests.Redot;

public readonly struct TestTrigger;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestDecorateUp()
    {
        var tree = new Tree();

        var container = new Node(tree);
        
        var node = new Node(tree);
        var node1 = new Node(tree);
        var node2 = new Node(tree);
        
        node.AddChild(node1);
        node.AddChild(node2);
        
        container.AddChild(node);
        
        Assert.Multiple(() =>
        {
            Assert.That(node.GetChildren(), Has.Count.EqualTo(2));
            Assert.That(node1.GetParent(), Is.EqualTo(node));
            Assert.That(node2.GetParent(), Is.EqualTo(node));
            
            Assert.That(container.GetChildren(), Has.Count.EqualTo(1));
            Assert.That(node.GetParent(), Is.EqualTo(container));
        });
        
        node.DecorateUp(new Component(self =>
        {
            self.On<TestTrigger>(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(self.Unchecked.GetParent(), Is.EqualTo(container));
                    Assert.That(container.GetChildren().ToArray(), Is.EqualTo(new[] {self.Unchecked}));

                    Assert.That(self.Unchecked.GetChildren(), Is.EqualTo(new[] {node}));
                }); 
                Assert.Pass();
            });
        }));
        
        container.Call<TestTrigger>();
        Assert.Fail();
    }
    
    [Test]
    public void TestDecorateDown()
    {
        var tree = new Tree();

        var container = new Node(tree);
        
        var node = new Node(tree);
        var node1 = new Node(tree);
        var node2 = new Node(tree);
        
        node.AddChild(node1);
        node.AddChild(node2);
        
        container.AddChild(node);
        
        Assert.Multiple(() =>
        {
            Assert.That(node.GetChildren(), Has.Count.EqualTo(2));
            Assert.That(node1.GetParent(), Is.EqualTo(node));
            Assert.That(node2.GetParent(), Is.EqualTo(node));
            
            Assert.That(container.GetChildren(), Has.Count.EqualTo(1));
            Assert.That(node.GetParent(), Is.EqualTo(container));
        });
        
        node.DecorateDown(new Component(self =>
        {
            self.On<TestTrigger>(() =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(self.Unchecked.GetParent(), Is.EqualTo(node));
                    Assert.That(node.GetChildren().ToArray(), Is.EqualTo(new[] {self.Unchecked}));

                    Assert.That(self.Unchecked.GetChildren(), Has.Count.EqualTo(2));
                    Assert.That(node1.GetParent(), Is.EqualTo(self.Unchecked));
                    Assert.That(node2.GetParent(), Is.EqualTo(self.Unchecked));
                }); 
                Assert.Pass();
            });
        }));
        
        container.Call<TestTrigger>();
        Assert.Fail();
    }
    
    [Test]
    public void TestUndecorate()
    {
        var tree = new Tree();

        var container = new Node(tree);
        
        var node = new Node(tree);
        var node1 = new Node(tree);
        var node2 = new Node(tree);
        
        node.AddChild(node1);
        node.AddChild(node2);
        
        container.AddChild(node);
        
        Assert.Multiple(() =>
        {
            Assert.That(node.GetChildren(), Has.Count.EqualTo(2));
            Assert.That(node1.GetParent(), Is.EqualTo(node));
            Assert.That(node2.GetParent(), Is.EqualTo(node));
            
            Assert.That(container.GetChildren(), Has.Count.EqualTo(1));
            Assert.That(node.GetParent(), Is.EqualTo(container));
        });
        
        node.Undecorate();
        
        Assert.Multiple(() =>
        {
            Assert.That(node.StateIs(NodeState.QueuedToDeletion));
            Assert.That(node.GetChildren(), Is.Empty);
            Assert.That(node.GetParent(), Is.Null);
            
            Assert.That(container.GetChildren(), Has.Count.EqualTo(2));
            Assert.That(node1.GetParent(), Is.EqualTo(container));
            Assert.That(node2.GetParent(), Is.EqualTo(container));
        });
    }
    
    [Test]
    public void TestComponentBuildReturnsCorrectNumOfChildren()
    {
        IComponent comp = new Component(_ => { });
        Assert.That(comp.Build(new Node(new Tree())).Count(), Is.EqualTo(0));
        
        IComponent comp1 = new Component();
        Assert.That(comp1.Build(new Node(new Tree())).Count(), Is.EqualTo(0));
        
        IComponent comp2 = new Component(_ => new Component());
        Assert.That(comp2.Build(new Node(new Tree())).Count(), Is.EqualTo(1));
        
        IComponent comp3 = new Component(_ => [new Component(), new Component()]);
        Assert.That(comp3.Build(new Node(new Tree())).Count(), Is.EqualTo(2));
    }
    
    [Test]
    public void TestComponentIsFakeIterable()
    {
        var comp = new Component()
        {
            new Component(),
            new Component(),
            new Component(),
            new Component(),
        };
        Assert.That(comp.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public void TestNodeStateOnDisposeEffect()
    {
        var tree = new Tree();
        tree.AttachToRoot(new Component(compA =>
        {
            compA.UseEffect(() =>
            {
                return () =>
                {
                    Assert.That(compA.Unchecked.StateIs(NodeState.Freed), Is.EqualTo(true));
                    Assert.Pass();
                };
            }, []);
            compA.QueueFree();
        }));
        tree.Update();
        Assert.Fail();
    }
    
    [Test]
    public void TestFirstEnterTreeEffect()
    {
        var tree = new Tree();

        var node = new Node(tree);
        var node1 = new Node(tree);

        int counter = 0;
        
        node1.UseEffect(() =>
        {
            counter += 1;
            return () => { };
        }, new EffectDependencies([]));
        
        node.AddChild(node1);
        node.RemoveChild(node1);
        node.AddChild(node1);
        
        Assert.That(counter, Is.EqualTo(1));
    }
    
    [Test]
    public void TestOnDisposeEffectIsNotCalled()
    {
        var tree = new Tree();

        var node = new Node(tree);
        var node1 = new Node(tree);

        int counter = 0;
        
        node1.UseEffect(() =>
        {
            return () =>
            {
                counter += 1;
            };
        }, new EffectDependencies([]));
        
        node.AddChild(node1);
        node.RemoveChild(node1);
        
        Assert.That(counter, Is.EqualTo(0));
    }
    
    [Test]
    public void TestOnDisposeEffectIsCalled()
    {
        var tree = new Tree();

        var node = new Node(tree);
        var node1 = new Node(tree);

        int counter = 0;
        
        node1.UseEffect(() =>
        {
            return () =>
            {
                counter += 1;
            };
        }, new EffectDependencies([]));
        
        node.AddChild(node1);
        node1.QueueFree();
        tree.Update();
        
        Assert.That(counter, Is.EqualTo(1));
    }
}