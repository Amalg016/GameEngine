using GameEngine.Core.Platform;

namespace GameEngine.components
{
    public class TranslateGizmo : Gizmo
    {

        public TranslateGizmo(Sprite sprite) : base(sprite)
        {

        }

        public override void Load()
        {
            base.Load();
        }

        public override void EditorUpdate()
        {
            if (activeGameObject == null)
            {
                Window.gl.ClearColor(1, 0, 0, 1);
            }
            else
            {
                Window.gl.ClearColor(1, 1, 1, 1);
            }
            if (activeGameObject != null)
            {
                if (xAxisActive && !yAxisActive)
                {
                    activeGameObject.transform.position.X += InputManager.getWorldDx();
                }
                if (yAxisActive && !xAxisActive)
                {
                    activeGameObject.transform.position.Y += InputManager.getWorldDy();
                }
            }
            base.EditorUpdate();
        }

    }
}
