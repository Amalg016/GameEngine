using GameEngine.util;
using System.Numerics;

namespace GameEngine.components
{
    public class MouseControls : Component
    {
        GameObject holdingObject=null;
        private float debounceTime = .05f;
        private float debounce;
        Vector4 color;
        public MouseControls()
        {
            debounce = debounceTime;
        }
        public void pickUpObject(GameObject go)
        {
            if (holdingObject != null)
            {
                this.holdingObject.Destroy();
            }
            
            holdingObject = go;
            color = holdingObject.GetComponent<SpriteRenderer>().GetColor();
            Vector4 vector = color;
            vector.W = 0.8f;
            holdingObject.GetComponent<SpriteRenderer>().setColor(vector);
           Window.GetScene().addGameObjectToScene(holdingObject);
        } 
        void Place()
        {
            GameObject newObj=this.holdingObject.Copy();
            newObj.GetComponent<SpriteRenderer>().setColor(color);
            newObj.Serialize();
        }
        public override void Load()
        {
            
        }

        public override void EditorUpdate()
        {
            debounce-=Time.deltaTime;
            if(holdingObject != null&&debounce<=0)
            {
                holdingObject.transform.position.X = InputManager.GetOrthoX();
                holdingObject.transform.position.Y = InputManager.GetOrthoY();

                holdingObject.transform.position.X = ((int)(holdingObject.transform.position.X /Settings.Grid_Width)*Settings.Grid_Width)-Settings.Grid_Width/2;
                holdingObject.transform.position.Y = ((int)(holdingObject.transform.position.Y / Settings.Grid_Height) * Settings.Grid_Height)+ Settings.Grid_Height/2;
                if (InputManager.LMisPressed)
                {
                    Place();

                    debounce = debounceTime;
                }
                if (InputManager.isKeyPressed(Silk.NET.Input.Key.Escape))
                {
                    Window.GetGUISystem().GetPropertiesWindow().setActiveGameObject(null);
                    holdingObject.Destroy();
                    holdingObject = null;
                }
            }
        }
    }
}
