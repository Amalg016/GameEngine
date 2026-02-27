using GameEngine.Core.Application;
using GameEngine.Editor;
using GameEngine.Editor.Window;
using GameEngine.Rendering.Core;
using GameEngine.scenes;

namespace GameEngine
{
    /// <summary>
    /// Editor-specific engine core. Adds GUISystem, wires up editor windows, 
    /// and configures the scene initializer to LevelEditorSceneInitializer.
    /// </summary>
    class EditorGameEngineCore : GameEngineCore
    {
        private static EditorGameEngineCore _editorInstance;
        public static new EditorGameEngineCore Instance => _editorInstance ??= new EditorGameEngineCore();

        GUISystem guiSystem;

        protected EditorGameEngineCore() : base()
        {
            // Wire up scene initializer factory
            SceneManager.SceneInitializerFactory = () => new LevelEditorSceneInitializer();

            // Wire up selection context to PropertiesWindow
            SelectionContext.OnSelectionChanged += (go) => PropertiesWindow.setActiveGameObject(go);
        }

        protected override sceneInitializer CreateInitialScene()
        {
            return new LevelEditorSceneInitializer();
        }

        protected override void OnWindowLoad()
        {
            base.OnWindowLoad();
            guiSystem = new GUISystem(WindowManger.gl, WindowManger.window, WindowManger.input, RenderSystem.PickingTexture);
        }

        protected override void OnGui(Scene currentScene)
        {
            guiSystem.Update(currentScene);
        }

        protected override void OnWindowClosed()
        {
            guiSystem.Exit();
            base.OnWindowClosed();
        }
    }
}
