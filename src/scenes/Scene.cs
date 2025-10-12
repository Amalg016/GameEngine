using GameEngine.components;
using GameEngine.Physics2D;
using GameEngine.Rendering.Core;
using GameEngine.Serialization;
using GameEngine.Core.Utilities;
using Newtonsoft.Json;
using Silk.NET.OpenGL;
using GameEngine.ECS;
using GameEngine.Core.Application;

namespace GameEngine.scenes
{
    public class Scene
    {
        internal bool isRunning = false;
        public List<GameObject> sceneGameObjects = new List<GameObject>();
        // public Camera Camera;
        public Renderer renderer;
        protected GL gl { get { return WindowManger.gl; } }
        // protected bool LevelLoaded = false;
        sceneInitializer SceneInitializer;
        physics2D physics2D;
        static bool played = false;
        public Scene(sceneInitializer sceneInitializer)
        {
            this.SceneInitializer = sceneInitializer;
        }
        public virtual void init()
        {
            //  Camera = new Camera(new Vector3(0, 0, 0), 16/9);

            SceneInitializer.loadResources(this);
            if (!played)
            {
                AssetPool.LoadResources();
                played = true;
            }
        }

        public void Start()
        {
            SceneInitializer.Init(this);
            physics2D = new physics2D();
            renderer = new Renderer(gl);

            foreach (GameObject go in sceneGameObjects)
            {
                go.Load();
                this.renderer.Add(go);
                this.physics2D.Add(go);
            }
            isRunning = true;
        }
        public void addGameObjectToScene(GameObject g)
        {
            if (!isRunning)
            {
                sceneGameObjects.Add(g);
            }
            else
            {
                sceneGameObjects.Add(g);
                g.Load();
                this.renderer.Add(g);
                this.physics2D.Add(g);
            }
        }
        //  public Camera camera()
        //   {
        //   return Camera;
        //   }

        public virtual void Gui()
        {
            this.SceneInitializer.imgui();
        }

        public void LoadLevelResources(string path)
        {
            Path = path;
            string s;
            try
            {
                s = File.ReadAllText(path);
                if (s != null)
                {
                    int maxGoId = -1;
                    int maxCompId = -1;
                    List<GameObject> ss = JsonConvert.DeserializeObject<List<GameObject>>(s, new GameObjectDeserializer(), new ComponentDeserializer(), new ColliderDeserializer());
                    foreach (GameObject go in ss)
                    {

                        foreach (var c in go.getAllComponents())
                        {
                            //   Console.WriteLine("cou"  +item.getAllComponents().Count);
                            if (c.getUid() > maxCompId)
                            {
                                maxCompId = c.getUid();
                            }
                        }
                        if (go.getUid() > maxGoId)
                        {
                            maxGoId = go.getUid();
                        }
                        addGameObjectToScene(go);

                    }
                    maxGoId++;
                    maxCompId++;
                    GameObject.setIdCounter(maxGoId);
                    Component.init(maxCompId);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }


        static int no = 1;
        string Path = "";
        public void SaveResources()
        {
            string lvlSource;
            try
            {
                List<GameObject> ss = new List<GameObject>();
                foreach (GameObject item in sceneGameObjects)
                {
                    if (item.doSerialize())
                    {
                        ss.Add(item);
                    }
                }

                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                lvlSource = JsonConvert.SerializeObject(ss, Formatting.Indented, settings);


                if (Path == null)
                {
                    Path = "default";
                }
                // Write JSON to file
                File.WriteAllText(Path, lvlSource);
                Console.WriteLine(lvlSource);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public virtual void EditorUpdate()
        {
            isRunning = true;
            for (int i = 0; i < sceneGameObjects.Count; i++)
            {

                sceneGameObjects[i].EditorUpdate();

                if (sceneGameObjects[i].isDead)
                {
                    this.renderer.DestroyGameObject(sceneGameObjects[i]);
                    physics2D.DestroyGameObject(sceneGameObjects[(i)]);
                    sceneGameObjects.Remove(sceneGameObjects[i]);
                }
            }

            for (int i = 0; i < sceneGameObjects.Count; i++)
            {

                sceneGameObjects[i].EditorUpdate();

            }
            var moveSpeed = 10f * (float)Time.deltaTime;


        }

        public virtual void Update()
        {
            isRunning = true;
            this.physics2D.Update();



            for (int i = 0; i < sceneGameObjects.Count; i++)
            {

                sceneGameObjects[i].Update();

                if (sceneGameObjects[i].isDead)
                {
                    this.renderer.DestroyGameObject(sceneGameObjects[i]);
                    physics2D.DestroyGameObject(sceneGameObjects[(i)]);
                    sceneGameObjects.Remove(sceneGameObjects[i]);
                }
            }

            var moveSpeed = 100f * (float)Time.deltaTime;

        }
        public virtual void Render()
        {
            this.renderer.render();
        }

        public virtual void Exit()
        {
            SceneInitializer.Exit(this);
            physics2D.Dispose();
        }

        public void Destroy()
        {
            foreach (var item in sceneGameObjects)
            {
                item.Destroy();
            }
            for (int i = 0; i < sceneGameObjects.Count; i++)
            {

                sceneGameObjects[i].EditorUpdate();

                if (sceneGameObjects[i].isDead)
                {
                    this.renderer.DestroyGameObject(sceneGameObjects[i]);
                    physics2D.DestroyGameObject(sceneGameObjects[(i)]);
                    sceneGameObjects.Remove(sceneGameObjects[i]);
                }
            }
            // sceneGameObjects.Clear();   
        }
        public GameObject getGameObject(int v)
        {
            foreach (GameObject go in sceneGameObjects)
            {
                if (go.getUid() == v)
                {
                    return go;
                }
            }
            return null;
            //throw new Exception("null GameObject with that id");
        }
    }


}
