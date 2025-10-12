using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.Rendering.Core;
using GameEngine.scenes;
using ImGuiNET;
using System.Numerics;

namespace GameEngine.editor
{
    public class GameViewWindow : IEditorWindow
    {
        private static float leftX;
        private static float rightX;
        private static float topY;
        private static float bottomY;
        private static bool isPlaying = false;
        string IEditorWindow.Title => "Game View";

        unsafe void IEditorWindow.Render()
        {
            ImGui.Begin("Game View", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.MenuBar);

            ImGui.BeginMenuBar();
            if (ImGui.MenuItem("Play", "", isPlaying, !isPlaying))
            {
                isPlaying = true;
                EventSystem.notify(null, new Event(EventType.GameEngineStartPlay));
            }
            if (ImGui.MenuItem("Stop", "", !isPlaying, isPlaying))
            {
                isPlaying = false;
                EventSystem.notify(null, new Event(EventType.GameEngineStopPlay));
            }
            ImGui.EndMenuBar();
            Vector2 windowSize = getLargestSizeForViewport();
            Vector2 windowPos = getCeneteredPositionForViewport(windowSize);

            ImGui.SetCursorPos(windowPos);

            Vector2 topLeft = new Vector2();
            topLeft = ImGui.GetCursorScreenPos();
            topLeft.X -= ImGui.GetScrollX();
            topLeft.Y -= ImGui.GetScrollY();
            leftX = topLeft.X;
            bottomY = topLeft.Y;
            rightX = topLeft.X + windowSize.X;
            topY = topLeft.Y + windowSize.Y;
            uint textureid = RenderSystem.FrameBuffer.getTextureID();
            ImGui.Image((IntPtr)textureid, windowSize, new Vector2(0, 1f), new Vector2(1f, 0));

            InputManager.setGameViewPos(new Vector2(topLeft.X, topLeft.Y));
            InputManager.setGameViewsize(new Vector2(windowSize.X, windowSize.Y));
            if (SceneManager.scenePath.Extension == ".scene")
            {
                if (ImGui.BeginDragDropTarget())
                {
                    ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload("Content Browser Item");
                    if (payload.NativePtr != null)
                    {
                        EventSystem.notify(null, new Event(EventType.SaveLevel));
                        EventSystem.notify(null, new Event(EventType.LoadLevel));
                        Console.WriteLine("Payload accepted" + SceneManager.scenePath + "_");
                    }

                    ImGui.EndDragDropTarget();
                }
            }
            ImGui.End();

        }

        public static bool getWantCapture()
        {
            // return InputManager.GetX() >= leftX && InputManager.GetX() <= rightX && InputManager.GetY() >= bottomY && InputManager.GetY() <= topY;
            // Console.WriteLine("Inputx: " + pos.X + ", InputY: " + pos.Y + ", " + leftX + ", " + rightX + ", " + bottomY + ", " + topY);
            // return pos.X >= leftX && pos.X <= rightX && pos.Y >= bottomY && pos.Y <= topY;
            return InputManager.mouseX >= leftX && InputManager.mouseX <= rightX && InputManager.mouseY >= bottomY && InputManager.mouseY <= topY;
        }


        public static Vector2 GetFramebufferMousePos()
        {
            Vector2 screenMouse = new Vector2(InputManager.mouseX, InputManager.mouseY);

            // Convert to normalized [0,1] in our viewport
            float normX = (screenMouse.X - leftX) / (rightX - leftX);
            float normY = (screenMouse.Y - bottomY) / (topY - bottomY);

            // Flip Y
            // normY = 1.0f - normY;

            // Convert to framebuffer coordinates
            float fbX = normX * RenderSystem.FrameBuffer.GetWidth();
            float fbY = normY * RenderSystem.FrameBuffer.GetHeight();

            return new Vector2(fbX, fbY);
        }

        private static Vector2 getLargestSizeForViewport()
        {
            Vector2 windowSize = new Vector2();
            windowSize = ImGui.GetContentRegionAvail();
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float targetAspect = 16.0f / 9.0f; // Or get from framebuffer
            float aspectWidth = windowSize.X;
            float aspectHeight = (aspectWidth) / targetAspect;
            if (aspectHeight > windowSize.Y)
            {
                aspectHeight = windowSize.Y;
                aspectWidth = (aspectHeight) * targetAspect;
            }

            RenderSystem.PickingTexture?.Resize((uint)aspectWidth, (uint)aspectHeight);
            RenderSystem.FrameBuffer?.Resize((uint)aspectWidth, (uint)aspectHeight);

            return new Vector2(aspectWidth, aspectHeight);
        }
        private static Vector2 getCeneteredPositionForViewport(Vector2 aspectSIze)
        {
            Vector2 windowSize = new Vector2();
            windowSize = ImGui.GetContentRegionAvail();
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();

            float viewPortX = (windowSize.X / 2) - (aspectSIze.X / 2);
            float viewPortY = (windowSize.Y / 2) - (aspectSIze.Y / 2);

            return new Vector2(viewPortX + ImGui.GetCursorPosX(), viewPortY + ImGui.GetCursorPosY());
        }

    }
}
