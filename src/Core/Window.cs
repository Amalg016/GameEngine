using Silk.NET.Input;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using GameEngine.Core.Utilities;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.scenes;
using WINDOW = Silk.NET.Windowing.Window;
using GameEngine.Rendering.Core;
using GameEngine.Core.Platform;
using GameEngine.ECS;
using GameEngine.Editor;
using GameEngine.Core;

namespace GameEngine
{
    public class Window : Observer
    {
        static IWindow window = null;
        static IInputContext input;
        public static GL gl;
        public static int Height = 1080, Width = 1920;

        private SceneManager sceneManager;
        private TimeManager timeManager;

        GUISystem guiSystem;
        RenderSystem renderSystem;

        public void Init(params string[] args)
        {
            EventSystem.addObserver(this);
            var options = WindowOptions.Default;
            options.Title = "AJEngine";
            options.Size = new Vector2D<int>(Width, Height);
            options.API = GraphicsAPI.Default;
            sceneManager = new SceneManager();
            timeManager = new TimeManager();
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
            timeManager.Initialize();

            input = window.CreateInput();
            gl = window.CreateOpenGL();

            InputManager.onLoad(input);
            AssetPool assetPool = new AssetPool(gl);
            renderSystem = new RenderSystem();
            renderSystem.Initialize(gl, Width, Height);

            //Camera = Camera.Main;

            guiSystem = new GUISystem(gl, window, input, RenderSystem.PickingTexture);
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
            guiSystem.Update(SceneManager.CurrentScene);
            timeManager.Update();
        }

        private void OnWindowClosed()
        {
            guiSystem.Exit();
            sceneManager.ExitScene();
            AssetPool.SaveResources();
            renderSystem.OnExit();
        }

        public static float getTargetAspectRatio()
        {
            return 16 / 9;
        }
    }
}

