using GameEngine.observers;
using GameEngine.observers.events;
using ImGuiNET;

namespace GameEngine.Editor.Window
{
    public class MenuBar : IEditorWindow
    {
        string files;

        public string Title => "File";

        public void Render()
        {
            ImGui.BeginMenuBar();
            if (ImGui.BeginMenu(Title))
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
