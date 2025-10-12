using GameEngine.observers.events;
using GameEngine.components;
using GameEngine.editor;
namespace GameEngine.scenes
{
    public class SceneManager
    {
        private static Scene _currentScene;
        private static bool _runtimePlaying = false;
        private static DirectoryInfo _scenePath = new DirectoryInfo("Assets/Scenes/Lvl1.json");

        public static DirectoryInfo scenePath => _scenePath;
        public static Scene CurrentScene => _currentScene;
        public static bool RuntimePlaying => _runtimePlaying;

        public void ChangeScene(sceneInitializer sceneInitializer)
        {
            _currentScene?.Destroy();
            Component.ID_Counter = 0;
            PropertiesWindow.setActiveGameObject(null);
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
                    ChangeScene(new LevelEditorSceneInitializer());
                    break;
                case EventType.GameEngineStopPlay:
                    _runtimePlaying = false;
                    ChangeScene(new LevelEditorSceneInitializer());
                    break;
                case EventType.LoadLevel:
                    ChangeScene(new LevelEditorSceneInitializer());
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
