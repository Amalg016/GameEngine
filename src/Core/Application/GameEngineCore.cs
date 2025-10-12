using GameEngine.Core.Platform;
using GameEngine.Core.Utilities;
using GameEngine.ECS;
using GameEngine.Editor;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.Rendering.Core;
using GameEngine.scenes;

namespace GameEngine.Core.Application
{
    class GameEngineCore : Observer
    {
        private static GameEngineCore _instance;
        public static GameEngineCore Instance => _instance ??= new GameEngineCore();
        private SceneManager sceneManager;
        private TimeManager timeManager;

        private GameEngineCore()
        {
            sceneManager = new SceneManager();
            timeManager = new TimeManager();
        }

        GUISystem guiSystem;
        RenderSystem renderSystem;

        WindowManger window;

        public void Start()
        {
            EventSystem.addObserver(this);
            window = new WindowManger();
            window.OnLoad += OnWindowLoad;
            window.OnUpdate += OnWindowUpdate;
            window.OnClosing += OnWindowClosed;
            window.Init();
        }

        private void OnWindowLoad()
        {
            Console.WriteLine("Loaded");
            timeManager.Initialize();

            InputManager.onLoad(WindowManger.input);
            AssetPool assetPool = new AssetPool(WindowManger.gl);
            renderSystem = new RenderSystem();
            renderSystem.Initialize(WindowManger.gl, WindowManger.Width, WindowManger.Height);

            //Camera = Camera.Main;

            guiSystem = new GUISystem(WindowManger.gl, WindowManger.window, WindowManger.input, RenderSystem.PickingTexture);
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

        public void onNotify(GameObject obj, Event _event)
        {
            sceneManager.HandleEngineEvent(_event);
        }
    }
}
