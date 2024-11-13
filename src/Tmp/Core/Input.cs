using Raylib_cs;
using Tmp.Core.Comp;
using Tmp.Math;

namespace Tmp.Core;

public class Input(bool enabled)
{
    private readonly HashSet<KeyboardKey> _pressedKeys = [];
    private readonly HashSet<KeyboardKey> _releasedKeysThisFrame = [];

    private bool _enabled = enabled;

    public bool IsKeyJustPressed(KeyboardKey key) => _enabled && Raylib.IsKeyPressed(key);

    public bool IsKeyPressed(KeyboardKey key) => _enabled && Raylib.IsKeyDown(key);

    public bool IsKeyUp(KeyboardKey key) => _enabled && Raylib.IsKeyUp(key);

    public bool IsKeyJustReleased(KeyboardKey key) => _enabled && Raylib.IsKeyReleased(key);

    public void Enable(bool enabled)
    {
        _enabled = enabled;
    }
    
    internal void Propagate(INodeInit subTree)
    {
        if (!_enabled) return;
        var keyPressed = Raylib.GetKeyPressed();
        while (keyPressed != 0)
        {
            _pressedKeys.Add((KeyboardKey)keyPressed);
            keyPressed = Raylib.GetKeyPressed();
        }

        foreach (var key in _pressedKeys)
        {
            if (Raylib.IsKeyPressed(key))
            {
                subTree.Call(new InputEventKey(key, true, false));
            }
            else if (Raylib.IsKeyPressedRepeat(key))
            {
                subTree.Call(new InputEventKey(key, true, Raylib.IsKeyPressedRepeat(key)));
            }
            else if (Raylib.IsKeyReleased(key))
            {
                subTree.Call(new InputEventKey(key, false, false));
                _releasedKeysThisFrame.Add(key);
            }
        }

        var mouseDelta = Raylib.GetMouseDelta();
        if (mouseDelta.LengthSquared() > 0)
        {
            subTree.Call(new InputEventMouseMotion(new Vector2I((int)mouseDelta.X, (int)mouseDelta.Y)));
        }

        foreach (var key in _releasedKeysThisFrame)
        {
            _pressedKeys.Remove(key);
        }
        _releasedKeysThisFrame.Clear();
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
;