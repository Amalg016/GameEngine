using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.editor
{
    public class Debug
    {
       static List<string> logs=new List<string>();
        public void imgui()
        {
            ImGui.Begin("Console");
            if (ImGui.Button("Clear"))
            {
                logs.Clear();   
            }
            int i = 0;
            ImGui.ListBox("", ref i, logs.ToArray(), logs.Count,50);
            
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
