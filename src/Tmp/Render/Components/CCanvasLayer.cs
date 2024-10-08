﻿using Tmp.Core.Redot;
using Tmp.Math;
using Tmp.Math.Components;
using Tmp.Render.Util;

namespace Tmp.Render.Components;

public class CCanvasLayer() : Component(self =>
{
    var layer = new CanvasLayer();
    
    self.CreateContext(new СNode2DTransform
    {
        Local = Transform2D.Identity
    });
    self.CreateContext<ICanvasItemContainer>(layer);
    
    var container = self.Use<ICanvasLayerContainer>();
    self.UseEffect(() =>
    {
        container.Get().Add(layer);
        return () => container.Get().Remove(layer);
    }, [container]);
});