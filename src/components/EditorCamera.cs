using System.Numerics;
using GameEngine.Core.Application;
using GameEngine.Core.Math;
using GameEngine.Core.Platform;
using GameEngine.Editor.Window;
using GameEngine.Rendering.Core;

namespace GameEngine.components
{
    public class EditorCamera : Component
    {
        private float dragDebounce = 0.1f;
        private float sensitiviity = 30f;
        float scrollSensitivity = 1f;
        float lerpTime = 0;
        Camera levelEditorCamera = RenderSystem.MainCamera;
        private Vector2 clickOrgin;
        private bool reset;
        public EditorCamera(Camera LevelEditorCamera)
        {
            this.levelEditorCamera = LevelEditorCamera;
            this.clickOrgin = new Vector2();
        }
        public override void Load()
        {
            levelEditorCamera = RenderSystem.MainCamera;
        }
        public override void Update()
        {
            updateOrtho();
        }

        public void updateOrtho()
        {
            float currentX = InputManager.mouseX - InputManager.gameViewPosX;
            float currentY = InputManager.mouseY - InputManager.gameViewPosY;

            // Normalize to [0, 1] within the game viewport
            float normX = currentX / InputManager.gameViewSizeX;
            float normY = currentY / InputManager.gameViewSizeY;

            // Flip Y: screen Y goes down, world Y goes up
            normY = 1.0f - normY;

            // Compute the world-space dimensions visible by the camera
            Vector2 projSize = levelEditorCamera.getProjectionSize();
            float zoom = levelEditorCamera.getZoom();
            float aspectRatio = GameViewWindow.getTargetAspectRatio();
            float worldWidth = projSize.X * zoom * aspectRatio;
            float worldHeight = projSize.Y * zoom;

            // Map [0,1] to [0, worldWidth/Height] and add camera position
            float worldX = normX * worldWidth + levelEditorCamera.Position.X;
            float worldY = normY * worldHeight + levelEditorCamera.Position.Y;

            InputManager.setOrthoX(worldX);
            InputManager.setOrthoY(worldY);
        }

        public override void EditorUpdate()
        {

            updateOrtho();
            // updateOrthoY();
            if (InputManager.RMouse && dragDebounce > 0)
            {
                this.clickOrgin = new Vector2(InputManager.GetOrthoX(), InputManager.GetOrthoY());
                dragDebounce -= Time.deltaTime;
                return;
            }
            else if (InputManager.RMouse)
            {
                Vector2 mousePos = new Vector2(InputManager.GetOrthoX(), InputManager.GetOrthoY());
                Vector2 delta = mousePos - clickOrgin;
                delta = delta * Time.deltaTime;
                gameObject.transform.position = (gameObject.transform.position - (delta * sensitiviity));
                clickOrgin = Mathf.Lerp(clickOrgin, mousePos, Time.deltaTime);
            }

            if (dragDebounce <= 0 && !InputManager.RMouse)
            {
                dragDebounce = 0.1f;
            }
            if (InputManager.Num8)
            {
                //   float addValue=(float)Math.Pow(Math.Abs(scrollSensitivity),(double)(1 / levelEditorCamera.getZoom()));

                levelEditorCamera.AddZoom(scrollSensitivity * Time.deltaTime);
            }
            if (InputManager.Num2)
            {
                //    float addValue=-(float)Math.Pow(Math.Abs(scrollSensitivity),(double)(1 / levelEditorCamera.getZoom()));
                levelEditorCamera.AddZoom(-scrollSensitivity * Time.deltaTime);
            }
            levelEditorCamera.Position.X = gameObject.transform.position.X;
            levelEditorCamera.Position.Y = gameObject.transform.position.Y;

            if (InputManager.reset)
            {
                this.reset = true;
            }
            if (reset)
            {
                this.gameObject.transform.position = Mathf.Lerp(gameObject.transform.position, new Vector2(0, 0), lerpTime);
                levelEditorCamera.setZoom(levelEditorCamera.getZoom() + ((1 - levelEditorCamera.getZoom()) * lerpTime));
                lerpTime += 0.1f * Time.deltaTime * 0.222f;
                if (Math.Abs(levelEditorCamera.Position.X) <= .2f && Math.Abs(levelEditorCamera.Position.Y) <= .2f)
                {
                    lerpTime = 0;
                    levelEditorCamera.setZoom(1);
                    gameObject.transform.position = new Vector2(0, 0);
                    reset = false;
                }
            }

        }

    }
}
