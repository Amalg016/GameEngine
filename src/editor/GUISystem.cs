using GameEngine.Editor.Window;
using GameEngine.Rendering.IMGUI;
using GameEngine.Rendering.Core;
using GameEngine.scenes;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace GameEngine.Editor
{
    public class GUISystem
    {
        private List<IEditorWindow> _editorWindows = new();
        private PropertiesWindow _propertiesWindow;
        ImGuiLayer _imGuiLayer;

        public GUISystem(GL gL, IWindow w, IInputContext inputContext, PickingTexture pickingTexture)
        {
            this._propertiesWindow = new PropertiesWindow(pickingTexture);
            this._editorWindows.Add(new GameViewWindow());
            this._editorWindows.Add(this._propertiesWindow);
            this._editorWindows.Add(new SceneHierarchyWindow());
            this._editorWindows.Add(new ContentBrowserWindow());
            this._editorWindows.Add(new AnimationWindow());
            this._editorWindows.Add(new Debug());
            this._editorWindows.Add(new MenuBar());
            this._imGuiLayer = new ImGuiLayer(gL, w, inputContext);
        }

        public void Update(Scene currentScene)
        {
            _imGuiLayer.Update(Time.deltaTime);
            SetUpDockSpace();
            foreach (var window in _editorWindows)
            {
                window.Render();
            }
            currentScene?.Gui();
            _imGuiLayer.Render();
        }

        public void Exit()
        {
            _imGuiLayer.Dispose();
        }

        private void SetUpDockSpace()
        {
            ImGuiWindowFlags s = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(GameEngine.Window.Width, GameEngine.Window.Height));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            s |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;

            ImGui.Begin("dockSpace", s);
            ImGui.PopStyleVar(2);

            //DockSpace
            ImGui.DockSpace(ImGui.GetID("Dockspace"));
        }

        public PropertiesWindow GetPropertiesWindow()
        {
            return this._propertiesWindow;
        }
    }
}
