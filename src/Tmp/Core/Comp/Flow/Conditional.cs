namespace Tmp.Core.Comp.Flow;

// TODO replace signal
// public class Conditional : Component
// {
//     public required ISignal<bool> When { get; init; }
//
//     protected override Components Init(INodeInit self)
//     {
//         self.UseEffect(prev =>
//         {
//             var render = When.Value;
//             if (prev == render) return render;
//             
//             if (render)
//             {
//                 CreateChildrenAndMount(Children);   
//             }
//             else
//             {
//                 ClearChildren();
//             }
//             
//             return render;
//         }, false);
//         
//         return [];
//     }
// }