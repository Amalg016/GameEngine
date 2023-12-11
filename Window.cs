using Silk.NET.Input;
using System.Numerics;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Diagnostics;
using GameEngine.util;
using GameEngine.renderer;
using Shader = GameEngine.renderer.Shader;
using Texture = GameEngine.renderer.Texture;
using FrameBuffer = GameEngine.renderer.FrameBuffer;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.scenes;
using GameEngine.components;
using WINDOW=Silk.NET.Windowing.Window;

namespace GameEngine
{
    public class Window:Observer
    {
        static IWindow window = null;
        static IInputContext input;
        public static GL gl;
        private static IKeyboard primaryKeyboard;
        static uint program;
        static Camera Camera;
        static GUISystem guicontroller;
        public static int Height = 1080, Width = 1920;
        private static FrameBuffer framebuffer;
        public static PickingTexture pickingTexture;
        private static bool RuntimePlaying = false;
        public  void Init(params string[] args)
        {
            EventSystem.addObserver(this);
            var options = WindowOptions.Default;
            options.Title = "AJEngine";
            options.Size = new Vector2D<int>(Width, Height);
             options.API = GraphicsAPI.Default;
            window = WINDOW.Create(options);



            window.Load += OnWindowLoad;

            window.Update += OnWindowUpdate;
            window.Closing += OnWindowClosed;
            window.Resize += Resize;
            //    window.FramebufferResize += s =>
            //    {
            //        // Adjust the viewport to the new window size
            //        gl.Viewport(s);
            //    };

            window.Run();
        }
        public void onNotify(GameObject obj, Event _event)
        {
            switch (_event.Type) 
            {
                case EventType.GameEngineStartPlay:
                    RuntimePlaying=true;
                    currentScene.SaveResources();
                    Window.ChangeScene(new LevelEditorSceneInitializer());
                    return;
                case EventType.GameEngineStopPlay: 
                    RuntimePlaying = false;
                    Window.ChangeScene(new LevelEditorSceneInitializer());
                    return;
                case EventType.LoadLevel:
                    Window.ChangeScene(new LevelEditorSceneInitializer());
                    return;
                case EventType.SaveLevel:
                    currentScene.SaveResources();
                    return; 
            }

            if (_event.Type == EventType.GameEngineStartPlay)
            {
            }
            else if (_event.Type == EventType.GameEngineStopPlay)
            {

            }
        }
        private static void Resize(Vector2D<int> obj)
        {
            Height = obj.Y;
            Width = obj.X;
            //   Console.WriteLine(Height + "w" + Width);
        }


        static Stopwatch stopwatch;
        static float BeginTime;
        static Shader defaultShader;
        static Shader pickingShader;
        static Scene currentScene = null;
        public static DirectoryInfo scenePath = new DirectoryInfo("Assets/Scenes/Lvl1.json");
        static Texture texture;
        public static void ChangeScene(sceneInitializer sceneInitializer)
        {
            
            if(currentScene != null)
            {
                currentScene.Destroy();
               // Console.WriteLine("mem" + currentScene.sceneGameObjects[1].name);
                currentScene = null;
            }
                    guicontroller.GetPropertiesWindow().setActiveGameObject(null);
                    Component.ID_Counter = 0;
                    currentScene = new Scene(sceneInitializer);
                    currentScene.init();
                    currentScene.LoadLevelResources(scenePath.FullName);
                    currentScene.Start();            
        }
        public static Scene GetScene()
        {
            return Window.currentScene;
        }
        public static Camera camera;

        private static void OnWindowLoad()
        {

            stopwatch = new Stopwatch();
            stopwatch.Start();
            BeginTime = 0;
            input = window.CreateInput();
            gl = window.CreateOpenGL();

            primaryKeyboard = input.Keyboards.FirstOrDefault();

            InputManager.keyboard = primaryKeyboard;
            InputManager.mouse = input.Mice.FirstOrDefault();
            foreach (IMouse mouse in input.Mice)
            {
                mouse.Click += (cursor, button, pos) => { Console.WriteLine($"Clicked {pos} {cursor.Cursor}"); };
            };
            foreach (IKeyboard keyboard in input.Keyboards)
            {
                keyboard.KeyDown +=InputManager.kls;
            }

            AssetPool assetPool = new AssetPool(gl);
            gl.ClearColor(1, 1, 1, 1);


            gl.Enable(GLEnum.Blend);
            gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

            defaultShader = AssetPool.getShader("Assets/Shader/shader.vert", "Assets/Shader/shader.frag", "DefaultShader");

            camera = new Camera(new Vector3(0, 0, 0), 1);
            //Camera = Camera.Main;
            framebuffer = new FrameBuffer(1920, 1080);
            pickingTexture = new PickingTexture(1920, 1080);
            //     gl.Viewport(0, 0, 1920, 1080);
            guicontroller = new GUISystem(gl, window, input, pickingTexture);
            if(scenePath != null)
            {
                ChangeScene(new LevelEditorSceneInitializer());
            }

            guicontroller.Load(currentScene);
            pickingShader = AssetPool.getShader("Assets/Shader/pickingShader.vert", "Assets/Shader/pickingShader.frag", "pickingShader");

        }


        private static void OnWindowUpdate(double obj)
        {
            InputManager.Update();
            gl.Disable(GLEnum.Blend);
            pickingTexture.enableWriting();

          //  gl.Viewport(0, 0, 1920, 1100);
         //   gl.ClearColor(0, 0, 0, 0);
            gl.Clear((uint)GLEnum.ColorBufferBit | (uint)GLEnum.DepthBufferBit);

            // bind shader
            Renderer.bindShader(pickingShader);
            if (currentScene != null)
            {
                currentScene.Render();
            }



            pickingTexture.disableWriting();
            gl.Enable(GLEnum.Blend);

            DebugDraw.beginFrame();
            framebuffer.Bind();
            gl.ClearColor(1, 1, 1, 1);
            gl.Clear(ClearBufferMask.ColorBufferBit);
            if (currentScene != null)
            {


                DebugDraw.Draw();
                Renderer.bindShader(defaultShader);
                if (RuntimePlaying)
                {
                currentScene.Update();
                }
                else
                {
                    currentScene.EditorUpdate();
                }
                currentScene.Render();
            }
            
            framebuffer.UnBind();
            guicontroller.Update(currentScene);
            Time.time = (float)stopwatch.Elapsed.TotalSeconds;
            Time.deltaTime = Time.time - BeginTime;
            BeginTime = Time.time;
        }

        private static readonly float[] Vertices =
        {
            //X    Y      Z        R  G  B  A    u v
             0.5f,  0.5f, 0.0f,    1, 1, 1, 1,   1,0,
             0.5f, -0.5f, 0.0f,    1, 1, 1, 1,   1,1,
            -0.5f, -0.5f, 0.0f,    1, 1, 1, 1,   0,1,
            -0.5f,  0.5f, 0.5f,    1, 1, 1, 1,   0,0
        };

        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };



        private static void OnWindowClosed()
        {
            guicontroller.Exit();
            if (currentScene != null)
            {
                if (!RuntimePlaying)
                {
                  currentScene.SaveResources();
                }
                currentScene.Exit();
                AssetPool.SaveResources();
                DebugDraw.OnExit();
            }
            gl?.Dispose();
        }

        public static FrameBuffer getFrameBuffer()
        {
            return framebuffer;
        }
        public static float getTargetAspectRatio()
        {


            // GameviewWindow Script doesnt uses this function it can be integrated
            return 16 / 9;
            return 1;
            return Width / Height;
        }
        public static GUISystem GetGUISystem()
        {
            return guicontroller;
        }

        
    }
}

