using GameEngine.scenes;
using ImGuiNET;

namespace GameEngine.editor
{
    public class SceneHierarchyWindow
    {
        List<GameObject> list = new List<GameObject>();
        string Name = "Scene Heirachy";
        public void imgui()
        {
            ImGui.Begin("Scene Hierarchy");
            if (SceneManager.CurrentScene == null)
            {
                ImGui.End();
                return;
            }
            list = SceneManager.CurrentScene.sceneGameObjects;
            int index = 0;
            foreach (var item in list)
            {
                if (!item.doSerialize())
                {
                    continue;
                }
                bool treeNodeOpen = doTreeNode(item, index);
                if (ImGui.IsItemActive())
                {
                    if (PropertiesWindow.window.getActiveGameObject() != item)
                    {
                        PropertiesWindow.window.setActiveGameObject(item);
                    }
                }
                if (treeNodeOpen)
                {
                    ImGui.TreePop();
                }

                index++;
            }

            ImGui.End();
        }
        private unsafe bool doTreeNode(GameObject obj, int index)
        {
            int i = index;
            ImGui.PushID(index);
            bool treeNodeOpen = ImGui.TreeNodeEx(obj.name,
                ImGuiTreeNodeFlags.FramePadding |
                ImGuiTreeNodeFlags.OpenOnArrow |
                ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.Framed, obj.name);

            ImGui.PopID();
            if (ImGui.BeginDragDropSource())
            {
                ImGui.SetDragDropPayload(Name, (IntPtr)(&i), sizeof(int));

                ImGui.Text(obj.name);
                ImGui.EndDragDropSource();

            }
            if (ImGui.BeginDragDropTarget())
            {
                ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload(Name);
                if (payload.NativePtr != null)
                {
                    var dataPtr = (int*)payload.Data;
                    int src = dataPtr[0];
                    var srcItem = list[src];
                    Console.WriteLine("Payload accepted" + srcItem.name + "_");
                }

                ImGui.EndDragDropTarget();
            }

            return treeNodeOpen;
        }

    }
}
