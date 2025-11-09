using GameEngine.Core.Platform;
using GameEngine.Editor.Window;

namespace GameEngine.components
{
    public class ScaleGizmo : Gizmo
    {
        public ScaleGizmo(Sprite sprite) : base(sprite)
        {
        }
        public override void Load()
        {
            base.Load();
        }
        public override void EditorUpdate()
        {
            var activeGameObject = PropertiesWindow.getActiveGameObject();
            if (activeGameObject != null)
            {
                if (xAxisActive && !yAxisActive)
                {
                    activeGameObject.transform.scale.X += InputManager.getWorldDx();

                }
                else if (yAxisActive && !xAxisActive)
                {
                    activeGameObject.transform.scale.Y += InputManager.getWorldDy();

                }
            }
            base.EditorUpdate();
        }
    }
}
