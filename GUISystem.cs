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
        //    ImGuiController controller;

        private GL _gl;
        private IView _view;
        private IInputContext _input;
        public bool _frameBegun;
        private readonly List<char> _pressedChars = new List<char>();
        private IKeyboard _keyboard;
        public IntPtr Context;
        private PropertiesWindow propertiesWindow;
        private MenuBar menuBar;
        private SceneHierarchyWindow hierarchyWindow;
        private AnimationWindow animationWindow;
        private ContentBrowserWindow contentBrowser;
        private Debug debugWindow;
        public GUISystem(GL gL, IWindow w, IInputContext inputContext, PickingTexture pickingTexture)
        {
            GL = gL;
            window = w;
            input = inputContext;
            this.propertiesWindow = new PropertiesWindow(pickingTexture);
            this.menuBar = new MenuBar();
            this.hierarchyWindow = new SceneHierarchyWindow();
            this.animationWindow = new AnimationWindow();
            this.contentBrowser = new ContentBrowserWindow();
            this.debugWindow = new Debug();
        }


        ImGuiLayer controller;
        public void Load(Scene currentScene)
        {

            // controller = new ImGuiController(GL, window, input);
            controller = new ImGuiLayer(GL, window, input);
        }
        public void Update(Scene currentScene)
        {
            controller.Update(Time.deltaTime);
            SetUpDockSpace();
            currentScene?.Gui();
            GameViewWindow.imgui();
            //ImGui.ShowDemoWindow();
            propertiesWindow.update(currentScene);
            propertiesWindow.imgui();
            hierarchyWindow.imgui();
            animationWindow.imgui();
            contentBrowser.imgui();
            debugWindow.imgui();
            //ImGui.End();
            controller.Render();

        }
        public void Exit()
        {

            controller.Dispose();
            //  Dispose();
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
            menuBar.imgui();

        }
        public PropertiesWindow GetPropertiesWindow()
        {
            return this.propertiesWindow;
        }

    }
}
