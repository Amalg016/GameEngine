using GameEngine.Core.Platform;
using GameEngine.Core.Utilities;
using GameEngine.ECS;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.Rendering.Core;
using GameEngine.scenes;

namespace GameEngine.Core.Application
{
    public class GameEngineCore : Observer
    {
        private static GameEngineCore _instance;
        public static GameEngineCore Instance => _instance ??= new GameEngineCore();
        protected SceneManager sceneManager;
        private TimeManager timeManager;
        private InputManager inputManager;

        protected GameEngineCore()
        {
            sceneManager = new SceneManager();
            timeManager = new TimeManager();
            inputManager = new InputManager();
        }

        RenderSystem renderSystem;

        protected WindowManger window;

        public virtual void Start()
        {
            EventSystem.addObserver(this);
            window = new WindowManger();
            window.OnLoad += OnWindowLoad;
            window.OnUpdate += OnWindowUpdate;
            window.OnClosing += OnWindowClosed;
            window.Init();
        }

        protected virtual void OnWindowLoad()
        {
            Console.WriteLine("Loaded");
            timeManager.Initialize();

            inputManager.onLoad(WindowManger.input);
            AssetPool assetPool = new AssetPool(WindowManger.gl);
            renderSystem = new RenderSystem();
            renderSystem.Initialize(WindowManger.gl, WindowManger.Width, WindowManger.Height);

            sceneManager.ChangeScene(CreateInitialScene());
        }

        protected virtual sceneInitializer CreateInitialScene()
        {
            return null; // Subclasses must override to provide their scene
        }

        protected virtual void OnWindowUpdate(double obj)
        {
            inputManager.Update();
            renderSystem.RenderFrame(SceneManager.CurrentScene, SceneManager.RuntimePlaying);
            if (SceneManager.RuntimePlaying)
            {
                SceneManager.CurrentScene?.Update();
            }
            else
            {
                SceneManager.CurrentScene?.EditorUpdate();
            }
            OnGui(SceneManager.CurrentScene);
            timeManager.Update();
        }

        protected virtual void OnGui(Scene currentScene)
        {
            // No GUI in base engine — editor overrides this
        }

        protected virtual void OnWindowClosed()
        {
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
