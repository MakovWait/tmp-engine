using Raylib_cs;
using Tmp.Core.Plugins;
using Tmp.Core.Redot;
using Tmp.Core.Shelf;
using Tmp.Math;

namespace Tmp.Core;

public class Input
{
    private static readonly HashSet<KeyboardKey> PressedKeys = [];
    private static readonly HashSet<KeyboardKey> ReleasedKeysThisFrame = [];

    public bool IsKeyJustPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);

    public bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyDown(key);

    public bool IsKeyUp(KeyboardKey key) => Raylib.IsKeyUp(key);

    public bool IsKeyJustReleased(KeyboardKey key) => Raylib.IsKeyReleased(key);

    internal void Propagate(Tree tree)
    {
        var keyPressed = Raylib.GetKeyPressed();
        while (keyPressed != 0)
        {
            PressedKeys.Add((KeyboardKey)keyPressed);
            keyPressed = Raylib.GetKeyPressed();
        }

        foreach (var key in PressedKeys)
        {
            if (Raylib.IsKeyPressed(key))
            {
                tree.Call(new InputEventKey(key, true, false));
            }
            else if (Raylib.IsKeyPressedRepeat(key))
            {
                tree.Call(new InputEventKey(key, true, Raylib.IsKeyPressedRepeat(key)));
            }
            else if (Raylib.IsKeyReleased(key))
            {
                tree.Call(new InputEventKey(key, false, false));
                ReleasedKeysThisFrame.Add(key);
            }
        }

        var mouseDelta = Raylib.GetMouseDelta();
        if (mouseDelta.LengthSquared() > 0)
        {
            tree.Call(new InputEventMouseMotion(new Vector2I((int)mouseDelta.X, (int)mouseDelta.Y)));
        }

        foreach (var key in ReleasedKeysThisFrame)
        {
            PressedKeys.Remove(key);
        }
        ReleasedKeysThisFrame.Clear();
    }
}

public interface IInputEvent
{
}

public readonly record struct InputEventMouseMotion(Vector2I Velocity) : IInputEvent;

public readonly record struct InputEventKey(KeyboardKey KeyCode, bool Pressed, bool IsEcho) : IInputEvent
{
    public bool IsJustPressed() => Raylib.IsKeyPressed(KeyCode);

    public bool IsJustReleased() => Raylib.IsKeyReleased(KeyCode);
}

public class PluginInput() : PluginWrap<App>(new PluginAnonymous<App>("input")
{
    OnBuild = app =>
    {
        app.Shelf.Set(new Input());
    },
    
    OnFinish = app =>
    {
        app.Shelf.Inspect<Tree>(tree =>
        {
            var input = app.Shelf.Get<Input>();
            app.PreUpdate += () =>
            {
                input.Propagate(tree);
            };
            tree.OnInit += _ =>
            {
                tree.SetSingleton(input);
            };
        });
    }
});