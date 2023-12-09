using GameEngine.components;
using GameEngine.Physics2D.components;
using GameEngine.renderer;
using GameEngine.scenes;
using GameEngine.util;
using ImGuiNET;
using System;

namespace GameEngine.editor
{
    public class PropertiesWindow
    {
        protected GameObject activeGameobject = null;
        private PickingTexture pickingTexture;
        public static PropertiesWindow window;
        public PropertiesWindow(PickingTexture pickingTexture)
        {
            this.pickingTexture = pickingTexture;
            window = this;
        }
        public void update(Scene currentScene)
        {
            if ( !InputManager.isDragging&& InputManager.LMisPressed && GameViewWindow.getWantCapture())
            {
                int x = (int)InputManager.GetX();
                int y = (int)InputManager.GetY();
                Console.WriteLine(x + "x  " + y + "y " + pickingTexture.readPixel(x, y));
             //   int ID = pickingTexture.readPixel(x, y);
                int ID = Window.pickingTexture.readPixel(x, y);
                Console.WriteLine(ID);
                GameObject pickedObj= currentScene.getGameObject(ID); 
                if (pickedObj != null&&pickedObj.GetComponent<NonPickable>()==null)
                {
                    activeGameobject = pickedObj;
                }
                else if (pickedObj == null & !InputManager.isDragging)
                {
                    activeGameobject=null;  
                }
            }
        }
        public void imgui()
        {
            if (activeGameobject != null)
            {
                ImGui.Begin("Inspector");


             //   ImGui.Text($"Name  {this.activeGameobject.name}");
          //      ImGui.LabelText(this.activeGameobject.name,"Name");
                activeGameobject.IMGUI();
                if (ImGui.BeginPopupContextWindow("Component Adder"))
                {
                    if (ImGui.MenuItem("Add RigidBody"))
                    {
                        if (activeGameobject.GetComponent<Rigidbody2D>() == null)
                        {
                            activeGameobject.AddComponent(new Rigidbody2D());
                        }
                    }
                    if (ImGui.MenuItem("Add Circle Collider"))
                    {
                        if (activeGameobject.GetComponent<CircleCollider>() == null)
                        {
                            activeGameobject.AddComponent(new CircleCollider());
                        }
                    } 
                    if (ImGui.MenuItem("Add 2dBox Collider"))
                    {
                        if (activeGameobject.GetComponent<Box2DCollider>() == null&&activeGameobject.GetComponent<CircleCollider>()==null)
                        {
                            activeGameobject.AddComponent(new Box2DCollider());
                        }
                    }
                    if (ImGui.MenuItem("Add Animator"))
                    {
                        if (activeGameobject.GetComponent<Animator>() == null)                        {
                            activeGameobject.AddComponent(new Animator());
                        }
                    }

            //        if (ImGui.MenuItem("Add to Prefabs"))
            //        {
            //            if (!activeGameobject.isPrefab)
            //            {
            //                activeGameobject.isPrefab=true;
            //                AssetPool.Prefabs.Add(activeGameobject.getUid());    
            //            }
            //        }
                    ImGui.EndPopup();  
                }
                ImGui.End();
            }
      
        }
    
        public GameObject getActiveGameObject()
        {
            return activeGameobject;
        }

        public void setActiveGameObject(GameObject go)
        {
            activeGameobject = go;
        }
    }
}
