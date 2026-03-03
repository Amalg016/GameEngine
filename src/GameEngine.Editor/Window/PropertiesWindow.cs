using GameEngine.components;
using GameEngine.Core.Application;
using GameEngine.Core.Platform;
using GameEngine.ECS;
using GameEngine.Physics2D.components;
using GameEngine.Rendering.Core;
using GameEngine.scenes;
using GameEngine.Serialization;
using ImGuiNET;

namespace GameEngine.Editor.Window
{
    public class PropertiesWindow : IEditorWindow
    {
        private static GameObject activeGameobject = null;
        private PickingTexture pickingTexture;
        public static PropertiesWindow window;

        public string Title => "Inspector";

        public PropertiesWindow(PickingTexture pickingTexture)
        {
            this.pickingTexture = pickingTexture;
            window = this;
        }
        public void update(Scene currentScene)
        {
        }

        public static GameObject getActiveGameObject()
        {
            return activeGameobject;
        }

        public static void setActiveGameObject(GameObject go)
        {
            activeGameobject = go;
        }

        public void Render()
        {
            if (activeGameobject != null)
            {
                ImGui.Begin(this.Title);


                //   ImGui.Text($"Name  {this.activeGameobject.name}");
                //      ImGui.LabelText(this.activeGameobject.name,"Name");
                activeGameobject.IMGUI();
                EditorInspector.DrawGameObject(activeGameobject);
                if (ImGui.BeginPopupContextWindow("Component Adder"))
                {
                    foreach (var kvp in ComponentRegistry.GetAllRegistered())
                    {
                        string displayName = kvp.Key.Contains('.') 
                            ? kvp.Key.Substring(kvp.Key.LastIndexOf('.') + 1) 
                            : kvp.Key;

                        if (ImGui.MenuItem("Add " + displayName))
                        {
                            // Only add if the component isn't already on the object
                            var existing = activeGameobject.GetComponent(kvp.Value);
                            if (existing == null)
                            {
                                var component = (Component)Activator.CreateInstance(kvp.Value);
                                activeGameobject.AddComponent(component);
                            }
                        }
                    }
                    ImGui.EndPopup();
                }
                ImGui.End();
            }

            if (!InputManager.isDragging && InputManager.LMisPressed && GameViewWindow.getWantCapture())
            {
                var mousePos = GameViewWindow.GetFramebufferMousePos();
                int x = (int)mousePos.X;
                int y = (int)mousePos.Y;
                Console.WriteLine(x + "x  " + y + "y " + pickingTexture.readPixel(x, y));
                int ID = this.pickingTexture.readPixel(x, y);
                GameObject pickedObj = SceneManager.CurrentScene.getGameObject(ID);
                if (pickedObj != null && pickedObj.GetComponent<NonPickable>() == null)
                {
                    SelectionContext.ActiveGameObject = pickedObj;
                }
                else if (pickedObj == null & !InputManager.isDragging)
                {
                    SelectionContext.ActiveGameObject = null;
                }
            }

        }
    }
}
