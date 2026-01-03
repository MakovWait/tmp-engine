using Lofi2D.Core.Comp;

namespace Lofi2D.Tests.ComponentsTree;

public class TestTrait : Trait
{
    public int Value { get; set; }
}

public class AnotherTrait : Trait;

public class TraitTests
{
    [Test]
    public void AddTraitReturnsTheTrait()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            var trait = new TestTrait { Value = 42 };
            var returned = self.AddTrait(trait);

            Assert.That(returned, Is.SameAs(trait));
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }

    [Test]
    public void GetTraitReturnsAddedTrait()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            var trait = new TestTrait { Value = 123 };
            self.AddTrait(trait);

            var retrieved = self.GetTrait<TestTrait>();

            Assert.That(retrieved, Is.SameAs(trait));
            Assert.That(retrieved.Value, Is.EqualTo(123));
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }

    [Test]
    public void GetTraitThrowsWhenNotFound()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            Assert.Throws<KeyNotFoundException>(() =>
            {
                self.GetTrait<TestTrait>();
            });
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }

    [Test]
    public void TryGetTraitReturnsNullWhenNotFound()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            var result = self.TryGetTrait<TestTrait>();

            Assert.That(result, Is.Null);
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }

    [Test]
    public void HasTraitReturnsTrueWhenExists()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            self.AddTrait(new TestTrait());

            Assert.That(self.HasTrait<TestTrait>(), Is.True);
            Assert.That(self.HasTrait<AnotherTrait>(), Is.False);
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }

    [Test]
    public void RemoveTraitRemovesExistingTrait()
    {
        var tree = new Tree();

        tree.Build(new CFunc(self =>
        {
            self.AddTrait(new TestTrait());

            var removed = self.RemoveTrait<TestTrait>();

            Assert.That(removed, Is.True);
            Assert.That(self.HasTrait<TestTrait>(), Is.False);
            Assert.Pass();

            return [];
        }));

        Assert.Fail();
    }
}