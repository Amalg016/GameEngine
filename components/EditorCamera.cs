using System.Numerics;

namespace GameEngine.components
{
    public class EditorCamera : Component
    {
        private float dragDebounce = 0.1f;
        private float sensitiviity = 30f;
        float scrollSensitivity = 1f;
        float lerpTime = 0;
        Camera levelEditorCamera=Window.camera;
        private Vector2 clickOrgin;
        private bool reset;
        public EditorCamera(Camera LevelEditorCamera)
        {
            this.levelEditorCamera = LevelEditorCamera;
            this.clickOrgin = new Vector2();
        }
        public override void Load()
        {
           levelEditorCamera=Window.camera; 
        }
        public override void Update()
        {
           InputManager.calOrthoX();
           InputManager.calOrthoY();
        }
        public override void EditorUpdate()
        {

            InputManager.calOrthoX();
            InputManager.calOrthoY();
            if (InputManager.RMouse && dragDebounce > 0)
            {
                this.clickOrgin = new Vector2(InputManager.GetOrthoX(),InputManager.GetOrthoY());
                dragDebounce-=Time.deltaTime;
                return;
            }
            else if(InputManager.RMouse)
            {
                Vector2 mousePos = new Vector2(InputManager.GetOrthoX(), InputManager.GetOrthoY());
                Vector2 delta =mousePos-clickOrgin;
                delta = delta * Time.deltaTime;
                gameObject.transform.position = (gameObject.transform.position - (delta*sensitiviity));
                clickOrgin = Mathf.Lerp(clickOrgin, mousePos, Time.deltaTime); 
            }

            if (dragDebounce <= 0 && !InputManager.RMouse)
            {
                dragDebounce = 0.1f;
            }
            if (InputManager.Num8)
            {
             //   float addValue=(float)Math.Pow(Math.Abs(scrollSensitivity),(double)(1 / levelEditorCamera.getZoom()));
               
                levelEditorCamera.AddZoom(scrollSensitivity*Time.deltaTime);
            }
            if (InputManager.Num2)
            {
            //    float addValue=-(float)Math.Pow(Math.Abs(scrollSensitivity),(double)(1 / levelEditorCamera.getZoom()));
                levelEditorCamera.AddZoom(-scrollSensitivity*Time.deltaTime);
            }
            levelEditorCamera.Position.X = gameObject.transform.position.X;
            levelEditorCamera.Position.Y = gameObject.transform.position.Y;

            if (InputManager.reset)
            {
                this.reset = true;
            }
            if (reset)
            {
                this.gameObject.transform.position=  Mathf.Lerp(gameObject.transform.position, new Vector2(0,0), lerpTime);
                levelEditorCamera.setZoom(levelEditorCamera.getZoom() + ((1 - levelEditorCamera.getZoom()) * lerpTime));
                lerpTime += 0.1f * Time.deltaTime*0.222f;
                if(Math.Abs(levelEditorCamera.Position.X)<=.2f && Math.Abs(levelEditorCamera.Position.Y) <=.2f)
                {
                    lerpTime = 0;
                    levelEditorCamera.setZoom(1);
                    gameObject.transform.position = new Vector2(0,0);
                    reset = false;
                }
            }

        }
      
    }
}
