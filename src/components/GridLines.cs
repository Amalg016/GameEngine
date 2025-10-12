using GameEngine.Rendering.Core;
using GameEngine.Core.Utilities;
using System.Numerics;

namespace GameEngine.components
{
    public class GridLines : Component
    {

        public override void Load()
        {

        }

        public override void EditorUpdate()
        {
            Camera camera = Window.camera;
            Vector3 cameraPos = camera.Position;
            Vector2 projectionSize = camera.getProjectionSize();

            float firstX = ((int)(cameraPos.X / Settings.Grid_Width) - 1) * Settings.Grid_Height;
            float firstY = ((int)(cameraPos.Y / Settings.Grid_Height) - 1) * Settings.Grid_Width;

            int numVtLines = (int)(projectionSize.X * camera.getZoom() / Settings.Grid_Width) + 2;
            int numHzLines = (int)(projectionSize.Y * camera.getZoom() / Settings.Grid_Height) + 2;

            float height = (int)(projectionSize.Y * camera.getZoom()) + Settings.Grid_Height * 2;
            float width = (int)(projectionSize.X * camera.getZoom()) + Settings.Grid_Width * 2;

            int maxlines = Math.Max(numVtLines, numHzLines);
            Vector3 color = new Vector3(0f, 0f, 0f);
            for (int i = 0; i < maxlines; i++)
            {
                float x = firstX + (Settings.Grid_Width * i);
                float y = firstY + (Settings.Grid_Height * i);

                if (i < numVtLines)
                {
                    DebugDraw.addLine2D(new Vector2(x, firstY), new Vector2(x, firstY + height), color);
                }
                if (i < numHzLines)
                {
                    DebugDraw.addLine2D(new Vector2(firstX, y), new Vector2(firstX + width, y), color);
                }
            }
        }
    }
}
