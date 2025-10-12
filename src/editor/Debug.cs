using ImGuiNET;
using System.Numerics;

namespace GameEngine.editor
{
    public class Debug : IEditorWindow
    {
        static List<string> logs = new List<string>();

        public string Title => "Console";

        public void Render()
        {
            ImGui.Begin(Title);
            if (ImGui.Button("Clear"))
            {
                logs.Clear();
            }
            int i = 0;
            ImGui.ListBox("", ref i, logs.ToArray(), logs.Count, 50);

            ImGui.End();
        }

        public static void Log(string log)
        {
            logs.Add(log);
        }
        public static void Log(int no)
        {
            logs.Add(no.ToString());
        }
        public static void Log(float no)
        {
            logs.Add(no.ToString());
        }
        public static void Log(Vector2 n)
        {
            logs.Add($"X={n.X}; Y={n.Y}");
        }
        public static void Log(Vector3 n)
        {
            logs.Add($"X={n.X}; Y={n.Y}; Z={n.Z}");
        }
        public static void Log(Vector4 n)
        {
            logs.Add($"X={n.X}; Y={n.Y}; Z={n.Z}; W={n.W}");
        }
        public static void Log(object obj)
        {
            logs.Add(obj.ToString());
        }

    }
}
