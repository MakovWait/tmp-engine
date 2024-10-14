
// Console.WriteLine("ss");

using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;

Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint | ConfigFlags.ResizableWindow);
Raylib.InitWindow(1280, 800, "raylib-Extras-cs [ImGui] example - simple ImGui Demo");
Raylib.SetTargetFPS(144);

rlImGui.Setup(true);

while (!Raylib.WindowShouldClose())
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.DarkGray);

    rlImGui.Begin();
    ImGui.ShowDemoWindow();

    if (ImGui.Begin("Simple Window"))
    {
        ImGui.TextUnformatted("Icon text " + IconFonts.FontAwesome6.Book);
    }
    ImGui.End();
    rlImGui.End();

    Raylib.EndDrawing();
}

rlImGui.Shutdown();
Raylib.CloseWindow();