using GameEngine.observers.events;
using GameEngine.components;
using GameEngine.Core.Application;
namespace GameEngine.scenes
{
    public class SceneManager
    {
        private static Scene _currentScene;
        private static bool _runtimePlaying = false;
        private static DirectoryInfo _scenePath = new DirectoryInfo("Assets/Scenes/Lvl1.json");

        /// <summary>
        /// Factory to create the scene initializer. Set by the editor or game project.
        /// </summary>
        public static Func<sceneInitializer> SceneInitializerFactory { get; set; }

        public static DirectoryInfo scenePath => _scenePath;
        public static Scene CurrentScene => _currentScene;
        public static bool RuntimePlaying => _runtimePlaying;

        public void ChangeScene(sceneInitializer sceneInitializer)
        {
            _currentScene?.Destroy();
            Component.ID_Counter = 0;
            SelectionContext.ActiveGameObject = null;
            _currentScene = new Scene(sceneInitializer);
            _currentScene.init();
            _currentScene.LoadLevelResources(_scenePath.FullName);
            _currentScene.Start();
        }

        public void HandleEngineEvent(Event gameEvent)
        {
            switch (gameEvent.Type)
            {
                case EventType.GameEngineStartPlay:
                    _runtimePlaying = true;
                    _currentScene?.SaveResources();
                    if (SceneInitializerFactory != null)
                        ChangeScene(SceneInitializerFactory());
                    break;
                case EventType.GameEngineStopPlay:
                    _runtimePlaying = false;
                    if (SceneInitializerFactory != null)
                        ChangeScene(SceneInitializerFactory());
                    break;
                case EventType.LoadLevel:
                    if (SceneInitializerFactory != null)
                        ChangeScene(SceneInitializerFactory());
                    break;
                case EventType.SaveLevel:
                    _currentScene?.SaveResources();
                    break;
            }
        }

        public void SaveCurrentScene()
        {
            if (!_runtimePlaying)
            {
                _currentScene?.SaveResources();
            }
        }

        public void ExitScene()
        {
            SaveCurrentScene();
            _currentScene?.Exit();
        }

        public static void ChangeScenePath(DirectoryInfo scenePath)
        {
            _scenePath = scenePath;
        }
    }
}
