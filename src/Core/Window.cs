using Silk.NET.Input;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Diagnostics;
using GameEngine.util;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.scenes;
using WINDOW = Silk.NET.Windowing.Window;
using GameEngine.Rendering;

namespace GameEngine
{
    public class Window : Observer
    {
        static IWindow window = null;
        static IInputContext input;
        public static GL gl;
        static uint program;
        static GUISystem guicontroller;
        public static int Height = 1080, Width = 1920;
        private static SceneManager sceneManager;

        static Stopwatch stopwatch;
        static float BeginTime;
        public static Camera camera;
        RenderSystem renderSystem;

        public void Init(params string[] args)
        {
            EventSystem.addObserver(this);
            var options = WindowOptions.Default;
            options.Title = "AJEngine";
            options.Size = new Vector2D<int>(Width, Height);
            options.API = GraphicsAPI.Default;
            sceneManager = new SceneManager();
            window = WINDOW.Create(options);


            window.Load += OnWindowLoad;
            window.Update += OnWindowUpdate;
            window.Closing += OnWindowClosed;
            window.Resize += Resize;
            window.Run();
        }
        public void onNotify(GameObject obj, Event _event)
        {
            sceneManager.HandleEngineEvent(_event);
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
            stopwatch = new Stopwatch();
            stopwatch.Start();
            BeginTime = 0;
            input = window.CreateInput();
            gl = window.CreateOpenGL();

            InputManager.onLoad(input);
            AssetPool assetPool = new AssetPool(gl);
            renderSystem = new RenderSystem();
            renderSystem.Initialize(gl, Width, Height);

            camera = new Camera(new Vector3(0, 0, 0), 1);
            //Camera = Camera.Main;

            guicontroller = new GUISystem(gl, window, input, RenderSystem.PickingTexture);
            sceneManager.ChangeScene(new LevelEditorSceneInitializer());
        }


        private void OnWindowUpdate(double obj)
        {
            InputManager.Update();
            renderSystem.RenderFrame(SceneManager.CurrentScene, SceneManager.RuntimePlaying);

            if (SceneManager.RuntimePlaying)
            {
                SceneManager.CurrentScene?.Update();
            }
            else
            {
                SceneManager.CurrentScene?.EditorUpdate();
            }
            guicontroller.Update(SceneManager.CurrentScene);
            Time.time = (float)stopwatch.Elapsed.TotalSeconds;
            Time.deltaTime = Time.time - BeginTime;
            BeginTime = Time.time;
        }

        private void OnWindowClosed()
        {
            guicontroller.Exit();
            sceneManager.ExitScene();
            AssetPool.SaveResources();
            renderSystem.OnExit();
        }

        public static float getTargetAspectRatio()
        {
            return 16 / 9;
        }

        public static GUISystem GetGUISystem()
        {
            return guicontroller;
        }
    }
}

