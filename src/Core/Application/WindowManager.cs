using Silk.NET.Input;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using WINDOW = Silk.NET.Windowing.Window;

namespace GameEngine.Core.Application
{
    public class WindowManger
    {
        public static IWindow window = null;
        public static IInputContext input;
        public static GL gl;
        public static int Height = 1080, Width = 1920;

        public event Action OnLoad;
        public event Action<double> OnUpdate;
        public event Action OnClosing;
        public void Init(params string[] args)
        {
            var options = WindowOptions.Default;
            options.Title = "AJEngine";
            options.Size = new Vector2D<int>(Width, Height);
            options.API = GraphicsAPI.Default;
            window = WINDOW.Create(options);


            window.Load += OnWindowLoad;
            window.Load += () => OnLoad?.Invoke();
            window.Update += (deltaTime) => OnUpdate?.Invoke(deltaTime);
            window.Closing += () => OnClosing?.Invoke();
            window.Resize += Resize;
            window.Run();
        }

        private void Resize(Vector2D<int> obj)
        {
            Height = obj.Y;
            Width = obj.X;
            // Update viewport
            gl.Viewport(0, 0, (uint)Width, (uint)Height);
        }

        private void OnWindowLoad()
        {
            input = window.CreateInput();
            gl = window.CreateOpenGL();
        }

        public static float getTargetAspectRatio()
        {
            return 16 / 9;
        }
    }
}

