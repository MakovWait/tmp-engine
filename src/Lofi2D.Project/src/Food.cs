using Lofi2D.Animation;
using Lofi2D.Asset.Components;
using Lofi2D.Core.Comp;
using Lofi2D.Core.Comp.Flow;
using Lofi2D.Math;
using Lofi2D.Math.Components;
using Lofi2D.Render.Components;
using Lofi2D.Render.Texture;

namespace Lofi2D.Project;

internal class Food(Bounds bounds, Snake snake)
{
    private readonly ReactiveList<FoodItem> _items = [];
    public Signal<IReadOnlyList<FoodItem>> ItemsChanged => _items.Changed;

    public void Update()
    {
        var spawnNextFood = false;
        foreach (var foodItem in _items)
        {
            if (foodItem.Position.DistanceSquaredTo(snake.Head.Transform.Origin) < 8 * 8)
            {
                _items.QueueRemove(foodItem);
                spawnNextFood = true;
                snake.AddBodyPart();
            }
        }
        _items.FlushRemoveQueue();
        if (spawnNextFood)
        {
            SpawnNextFood();
        }
    }
    
    public void SpawnNextFood()
    {
        _items.Add(new FoodItem(Guid.NewGuid().ToString())
        {
            Position = bounds.GetRandomFoodPosition()
        });
    }  
}

public class CFood : Component
{
    protected override Components Init(INodeInit self)
    {
        var food = self.UseContext<Food>();

        self.OnMount(food.SpawnNextFood);
        self.On<Update>(_ => food.Update());

        return new CFor<FoodItem>()
        {
            In = food.ItemsChanged,
            ItemKey = item => item.Key,
            Render = (item, _) => new CFoodItem(item)
        };
    }
}

public class FoodItem(string key)
{
    public string Key { get; } = key;

    public required Vector2 Position { get; init; }
}

public class CFoodItem(FoodItem data) : Component
{
    protected override Components Init(INodeInit self)
    {
        var transform = self.UseTransform2D(new Transform2D(0, data.Position));
        var canvasItem = self.UseCanvasItem(transform);
        var texture = self.UseAsset<ITexture2D>("assets://body_part_texture.jass");

        var tween = self.CreateOneShotTween();
        tween.SetEase(Easing.EaseType.Out).SetTrans(Easing.TransitionType.Back);
        tween.TweenMethod(transform.SetScale, Vector2.Zero, Vector2.One, 0.3f);
        
        canvasItem.OnDraw(
            ctx =>
            {
                texture.Value.Draw(ctx, new Vector2(-16, -16), Colors.Red);
            }
        );

        return base.Init(self);
    }
}