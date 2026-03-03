using GameEngine.components;
using GameEngine.components.Animations;
using GameEngine.Core.Application;
using GameEngine.Editor;
using GameEngine.Editor.Window;
using GameEngine.Physics2D.components;
using GameEngine.Rendering.Core;
using GameEngine.scenes;
using GameEngine.Serialization;
using System.Diagnostics;
using System.Reflection;

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

            // Register built-in engine components for the "Add Component" menu
            ComponentRegistry.Register<Rigidbody2D>();
            ComponentRegistry.Register<Box2DCollider>();
            ComponentRegistry.Register<CircleCollider>();
            ComponentRegistry.Register<Animator>();
            ComponentRegistry.Register<SpriteRenderer>();

            // Auto-compile and load game scripts
            LoadScripts();
        }

        private void LoadScripts()
        {
            // Find the Scripts project relative to the solution root
            // Editor exe is at: src/GameEngine.Editor/bin/x64/Debug/net8.0/
            // Solution root is: ../../../../../../
            string baseDir = AppContext.BaseDirectory;
            string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "..", ".."));
            string scriptsCsproj = Path.Combine(solutionRoot, "Scripts", "Scripts.csproj");

            if (!File.Exists(scriptsCsproj))
            {
                Console.WriteLine($"No Scripts project found at: {scriptsCsproj}");
                Console.WriteLine("Create a Scripts/ folder with a Scripts.csproj to add custom components.");
                return;
            }

            Console.WriteLine("Compiling game scripts...");

            // Build the scripts project
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{scriptsCsproj}\" -c Release -p:Platform=x64 --nologo -v q",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine("Script compilation failed:");
                Console.WriteLine(output);
                Console.WriteLine(errors);
                return;
            }

            Console.WriteLine("Scripts compiled successfully!");

            // Load the built DLL
            string scriptsDll = Path.Combine(solutionRoot, "Scripts", "bin", "x64", "Release", "net8.0", "Scripts.dll");
            if (File.Exists(scriptsDll))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(scriptsDll);
                    ComponentRegistry.RegisterAllFromAssembly(assembly);
                    
                    var registered = ComponentRegistry.GetAllRegistered();
                    Console.WriteLine($"Loaded {registered.Count} component types from scripts.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed to load scripts DLL: {e.Message}");
                }
            }
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
