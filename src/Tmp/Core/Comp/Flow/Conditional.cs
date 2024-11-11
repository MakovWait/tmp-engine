namespace Tmp.Core.Comp.Flow;

public class Conditional : Component
{
    public required ISignal<bool> When { get; init; }

    protected override Components Init()
    {
        Self.UseEffect(prev =>
        {
            var render = When.Value;
            if (prev == render) return render;
            
            if (render)
            {
                CreateChildrenAndMount(Children);   
            }
            else
            {
                ClearChildren();
            }
            
            return render;
        }, false);
        
        return [];
    }
}