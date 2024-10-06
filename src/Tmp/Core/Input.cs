using Raylib_cs;
using Tmp.Core.Redot;
using Tmp.Math;

namespace Tmp.Core;

public static class Input
{
    private static readonly HashSet<KeyboardKey> PressedKeys = [];
    private static readonly HashSet<KeyboardKey> ReleasedKeysThisFrame = [];

    public static bool IsKeyJustPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);

    public static bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyDown(key);

    public static bool IsKeyUp(KeyboardKey key) => Raylib.IsKeyUp(key);

    public static bool IsKeyJustReleased(KeyboardKey key) => Raylib.IsKeyReleased(key);

    static internal void Propagate(Tree tree)
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