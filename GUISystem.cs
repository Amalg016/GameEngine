using GameEngine.editor;
using GameEngine.IMGUI;
using GameEngine.renderer;
using GameEngine.scenes;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace GameEngine
{
    public class GUISystem
    {
        GL GL;
        IWindow window;
        IInputContext input;
        private List<IEditorWindow> _editorWindows = new();
        private GL _gl;
        private IView _view;
        private IInputContext _input;
        public bool _frameBegun;
        private readonly List<char> _pressedChars = new();
        private IKeyboard _keyboard;
        public IntPtr Context;
        private PropertiesWindow propertiesWindow;
        ImGuiLayer controller;

        public GUISystem(GL gL, IWindow w, IInputContext inputContext, PickingTexture pickingTexture)
        {
            GL = gL;
            window = w;
            input = inputContext;
            this.propertiesWindow = new PropertiesWindow(pickingTexture);
            this._editorWindows.Add(new GameViewWindow());
            this._editorWindows.Add(this.propertiesWindow);
            this._editorWindows.Add(new SceneHierarchyWindow());
            this._editorWindows.Add(new ContentBrowserWindow());
            this._editorWindows.Add(new AnimationWindow());
            this._editorWindows.Add(new Debug());
            this._editorWindows.Add(new MenuBar());
        }


        public void Load(Scene currentScene)
        {
            controller = new ImGuiLayer(GL, window, input);
        }
        public void Update(Scene currentScene)
        {
            controller.Update(Time.deltaTime);
            SetUpDockSpace();
            foreach (var window in _editorWindows)
            {
                window.Render();
            }
            currentScene?.Gui();
            propertiesWindow.update(currentScene);
            controller.Render();
        }
        public void Exit()
        {
            controller.Dispose();
        }


        private void SetUpDockSpace()
        {
            ImGuiWindowFlags s = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoDocking;

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(Window.Width, Window.Height));
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
            return this.propertiesWindow;
        }

    }
}
