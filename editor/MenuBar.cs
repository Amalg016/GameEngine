using GameEngine.observers;
using GameEngine.observers.events;
using ImGuiNET;
using System.IO;
using System.Security.Permissions;

namespace GameEngine.editor
{
    public class MenuBar
    {
        string files;
        public void imgui()
        {
            ImGui.BeginMenuBar();
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Save", "Ctrl+S"))
                {
                    EventSystem.notify(null, new Event(EventType.SaveLevel));
                }
                if (ImGui.MenuItem("Load", "Ctrl+L"))
                {
                    EventSystem.notify(null, new Event(EventType.LoadLevel));
                }
                if (ImGui.MenuItem("Open", "Ctrl+O"))
                {
                       
                }
                ImGui.EndMenu();
            }

            ImGui.EndMenuBar();
        }
    }
}
