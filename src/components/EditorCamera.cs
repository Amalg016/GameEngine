using System.Numerics;
using GameEngine.Core.Application;
using GameEngine.Core.Math;
using GameEngine.Core.Platform;
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

            // Convert to NDC
            float ndcX = (currentX / InputManager.gameViewSizeX) * 2.0f - 1.0f;
            float ndcY = 1.0f - (currentY / InputManager.gameViewSizeY) * 2.0f;
            // Use a single point for both coordinates

            Vector4 ndcPos = new Vector4(ndcX, ndcY, 0, 1);
            Matrix4x4 inverseViewProj = levelEditorCamera.GetInverseProj() * levelEditorCamera.GetInverseView();
            Vector4 worldPos = Mathf.Multiply(inverseViewProj, ndcPos);

            InputManager.setOrthoX(worldPos.X);
            InputManager.setOrthoY(worldPos.Y);
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
