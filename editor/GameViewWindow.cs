using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.scenes;
using ImGuiNET;
using System.Numerics;

namespace GameEngine.editor
{
    public static class GameViewWindow
    {
        private static float leftX;
        private static float rightX;
        private static float topY;
        private static float bottomY;
        private static bool isPlaying = false;
        public unsafe static void imgui()
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
            uint textureid = Window.getFrameBuffer().getTextureID();
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
            return InputManager.GetX() >= leftX && InputManager.GetX() <= rightX && InputManager.GetY() >= bottomY && InputManager.GetY() <= topY;
        }

        private static Vector2 getLargestSizeForViewport()
        {
            Vector2 windowSize = new Vector2();
            windowSize = ImGui.GetContentRegionAvail();
            windowSize.X -= ImGui.GetScrollX();
            windowSize.Y -= ImGui.GetScrollY();


            float aspectWidth = windowSize.X;
            float aspectHeight = (aspectWidth) / (16 / 9);
            if (aspectHeight > windowSize.Y)
            {
                aspectHeight = windowSize.Y;
                aspectWidth = (aspectHeight) * 16 / 9;
                // Console.WriteLine(aspectHeight + "H   " + aspectWidth + "W");
            }

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
