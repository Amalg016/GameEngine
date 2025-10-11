using System.Numerics;
using Silk.NET.Input;

namespace GameEngine
{
    public static class InputManager
    {
        public static IKeyboard keyboard;
        public static IMouse mouse;
        public static float mouseX { get { return mouse.Position.X; } }
        public static float mouseY { get { return mouse.Position.Y; } }
        public static float worldX, worldY, lastWorldX, lastWorldY;


        public static bool V1;
        public static bool H1;
        public static bool V2;
        public static bool H2;

        public static bool E;
        public static bool R;

        public static bool Num8;
        public static bool Num2;
        public static bool reset;
        public static bool duplPressed;
        public static bool duplicate;
        public static bool delete;

        public static bool isDragging;
        public static bool LMouse;

        public static bool RMouse;
        public static bool mMouse;
        public static bool LMisPressed;
        private static bool LMisReady;
        private static bool DupReady;
        private static Vector2 gameViewPos = new Vector2();
        public static float gameViewPosX { get { return gameViewPos.X; } }
        public static float gameViewPosY { get { return gameViewPos.Y; } }
        private static Vector2 gameViewSize = new Vector2();
        public static float gameViewSizeX { get { return gameViewSize.X; } }
        public static float gameViewSizeY { get { return gameViewSize.Y; } }
        public static bool[] keyPressed = new bool[350];
        public static bool[] keyBeginPressed = new bool[350];

        public static void kls(IKeyboard arg1, Key arg2, int arg3)
        {
            keyPressed[(int)arg2] = keyboard.IsKeyPressed(arg2) ? true : false; ;
            keyBeginPressed[(int)arg2] = keyboard.IsKeyPressed(arg2) ? true : false;
        }

        public static void Update()
        {
            LMisPressed = false;
            duplPressed = false;
            if (LMouse)
            {
                isDragging = true;
            }
            else { isDragging = false; }
            V1 = (keyboard.IsKeyPressed(Key.Up)) ? true : false;
            H1 = (keyboard.IsKeyPressed(Key.Left)) ? true : false;
            V2 = (keyboard.IsKeyPressed(Key.Down)) ? true : false;
            H2 = (keyboard.IsKeyPressed(Key.Right)) ? true : false;
            E = (keyboard.IsKeyPressed(Key.E)) ? true : false;
            R = (keyboard.IsKeyPressed(Key.R)) ? true : false;
            Num8 = (keyboard.IsKeyPressed(Key.Keypad8)) ? true : false;
            Num2 = (keyboard.IsKeyPressed(Key.Keypad2)) ? true : false;
            reset = (keyboard.IsKeyPressed(Key.Q) && keyboard.IsKeyPressed(Key.ControlLeft)) ? true : false;
            duplicate = (keyboard.IsKeyPressed(Key.D) && keyboard.IsKeyPressed(Key.ControlLeft)) ? true : false;
            LMouse = (mouse.IsButtonPressed(MouseButton.Left)) ? true : false;
            RMouse = (mouse.IsButtonPressed(MouseButton.Right)) ? true : false;
            mMouse = (mouse.IsButtonPressed(MouseButton.Middle)) ? true : false;
            delete = isKeyPressed(Key.Delete);

            if (LMouse && !LMisReady)
            {
                LMisPressed = true;
                LMisReady = true;
            }
            if (!LMouse)
            {
                LMisReady = false;
            }
            if (duplicate && !DupReady)
            {
                duplPressed = true;
                DupReady = true;
            }
            if (!duplicate)
            {
                DupReady = false;
            }

            lastWorldX = worldX;
            lastWorldY = worldY;

        }

        public static float getWorldDx()
        {
            return worldX - lastWorldX;
        }
        public static float getWorldDy()
        {
            return worldY - lastWorldY;
        }
        public static void setGameViewPos(Vector2 viewpos)
        {
            gameViewPos.X = viewpos.X;
            gameViewPos.Y = viewpos.Y;
        }
        public static void setGameViewsize(Vector2 viewSize)
        {
            gameViewSize.X = viewSize.X;
            gameViewSize.Y = viewSize.Y;
        }
        public static bool isKeyPressed(Key key)
        {
            return (keyboard.IsKeyPressed(key)) ? true : false;
        }

        public static float GetX()
        {
            float currentX = mouseX - gameViewPos.X;
            currentX = (currentX / gameViewSize.X) * 1920;
            return currentX;

        }
        public static float GetY()
        {
            float currentY = mouseY - gameViewPos.Y;
            currentY = 1080 - ((currentY / gameViewSize.Y) * 1080);
            return currentY;
        }

        public static void setOrthoX(float x)
        {
            worldX = x;
        }

        public static void setOrthoY(float y)
        {
            worldY = y;
        }

        public static float GetOrthoX()
        {
            return worldX;
        }

        public static float GetOrthoY()
        {
            return worldY;
        }
    }
}
