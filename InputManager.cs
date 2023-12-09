using System;
using System.Buffers.Binary;
using System.Numerics;
using Silk.NET.Input;
using Silk.NET.Input.Extensions;

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
        private static Vector2 gameViewSize = new Vector2();
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
            if ( LMouse)
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
                reset=(keyboard.IsKeyPressed(Key.Q)&& keyboard.IsKeyPressed(Key.ControlLeft)) ? true : false;
                duplicate=(keyboard.IsKeyPressed(Key.D)&& keyboard.IsKeyPressed(Key.ControlLeft)) ? true : false;
                LMouse=(mouse.IsButtonPressed(MouseButton.Left))? true : false;
                RMouse=(mouse.IsButtonPressed(MouseButton.Right))? true : false;
                mMouse=(mouse.IsButtonPressed(MouseButton.Middle))? true : false;
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
                DupReady= false;
            }

            lastWorldX = worldX;
            lastWorldY = worldY;
        
        }

        public static void calOrthoY()
        {
            float currentY = mouseY - gameViewPos.Y;
            currentY = -((currentY / gameViewSize.Y) * 2.0f - 1.0f);
            Vector4 tmp = new Vector4(0, currentY, 0, 1);

            Matrix4x4 viewProjection = new Matrix4x4();
            Camera camera = Window.camera;
            viewProjection = camera.GetInverseProj() * camera.GetInverseView();
            tmp = InputManager.Multiply(viewProjection, tmp);
            //  tmp= InputManager.Multiply(Program.GetScene().Camera.GetInverseProj(), tmp);
            //  tmp=InputManager.Multiply(Program.GetScene().Camera.GetInverseView(), tmp);
            currentY = tmp.Y;
            worldY = tmp.Y;
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
            currentX = (currentX / gameViewSize.X) *1920;
            return currentX;

        }
        public static float GetY()
        {
            float currentY = mouseY - gameViewPos.Y;
            currentY = 1080-((currentY / gameViewSize.Y) * 1080);
            return currentY;
        }
        public static void calOrthoX()
        {
            float currentX = mouseX - gameViewPos.X;
            //   Console.WriteLine(gameViewPos.X+"y is"+gameViewPos.Y);  
            // currentX = (currentX / (float)Program.Width)*2f -1;
            currentX = (currentX / gameViewSize.X) * 2.0f - 1.0f;
            Vector4 tmp = new Vector4(currentX, 0, 0, 1);
            Matrix4x4 viewProjection = new Matrix4x4();

            Camera camera = Window.camera;
            viewProjection = camera.GetInverseProj() * camera.GetInverseView();
            tmp = InputManager.Multiply(viewProjection, tmp);
            
            //currentX = tmp.X;
           worldX = tmp.X;
        }
        public static float GetOrthoX()
        {
            return worldX;
        }
        public static float GetOrthoY()
        {
       //     float currentY = Program.Height- mouseY;
                         
            return worldY;
        }
        public static Vector4 Multiply(Matrix4x4 matrix, Vector4 vector)
        {
            return new Vector4(
                matrix.M11 * vector.X + matrix.M21 * vector.Y + matrix.M31 * vector.Z + matrix.M41 * vector.W,
                matrix.M12 * vector.X + matrix.M22 * vector.Y + matrix.M32 * vector.Z + matrix.M42 * vector.W,
                matrix.M13 * vector.X + matrix.M23 * vector.Y + matrix.M33 * vector.Z + matrix.M43 * vector.W,
                matrix.M14 * vector.X + matrix.M24 * vector.Y + matrix.M34 * vector.Z + matrix.M44 * vector.W
                );
        }
        public static Vector4 Multipl(Matrix4x4 matrix, Vector4 vector)
        {
            return new Vector4(
                matrix.M11 * vector.X + matrix.M12 * vector.Y + matrix.M13 * vector.Z + matrix.M14 * vector.W,
                matrix.M21 * vector.X + matrix.M22 * vector.Y + matrix.M23 * vector.Z + matrix.M24 * vector.W,
                matrix.M31 * vector.X + matrix.M32 * vector.Y + matrix.M33 * vector.Z + matrix.M34 * vector.W,
                matrix.M41 * vector.X + matrix.M42 * vector.Y + matrix.M43 * vector.Z + matrix.M44 * vector.W
                );
        }



    }
}
