using Silk.NET.OpenGL;
using Shader = GameEngine.Rendering.Shader;
using Texture = GameEngine.Rendering.Texture;
using GameEngine.components;
using Newtonsoft.Json;
using GameEngine.components.Animations;
using GameEngine.Serialization;

namespace GameEngine.util
{
    public class AssetPool
    {
        public static GL GL;
        public AssetPool(GL gL)
        {
            AssetPool.GL = gL;
            //   AssetPool.shader = new Shader(AssetPool.GL, "Assets/Shader/shader.vert", "Assets/Shader/shader.frag");
        }
        // public static Shader shader;
        [JsonIgnore] private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();
        [JsonIgnore] private static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        [JsonIgnore] public static Dictionary<string, Spritesheet> spritesheets = new Dictionary<string, Spritesheet>();
        [JsonRequired] public static List<Animation> allAnimations = new List<Animation>();
        public static List<AnimationController> animationControllers = new List<AnimationController>();
        public static Dictionary<string, string> Prefabs = new Dictionary<string, string>();
        public static Shader getShader(string vertexpath, string fragmentpath, string resourceName)
        {
            // var root=Path.GetPathRoot(resourceName);
            if (shaders.ContainsKey(resourceName))
            {
                return shaders[resourceName];
            }
            Shader shader = new Shader(AssetPool.GL, vertexpath, fragmentpath);
            AssetPool.shaders.Add(resourceName, shader);
            return shader;
        }

        public static AnimationController TryFindController(string Name)
        {
            foreach (var item in animationControllers)
            {
                if (item.Name == Name)
                {
                    return item;
                }
            }
            return null;
        }

        public static Texture getTexture(string path, string resourceName)
        {
            // var root=Path.GetPathRoot(resourceName);

            if (textures.ContainsKey(resourceName))
            {
                return textures[resourceName];
            }
            Texture texture = new Texture(path);
            AssetPool.textures.Add(resourceName, texture);
            return texture;
        }
        public static Texture TryGetTexture(string resourceName)
        {
            // var root=Path.GetPathRoot(resourceName);

            if (textures.ContainsKey(resourceName))
            {
                return textures[resourceName];
            }
            return null;
        }
        public static Animation TryGetAnimation(string Name)
        {
            foreach (Animation animation in allAnimations)
            {
                if (animation.Name == Name)
                {
                    return animation;
                }
            }
            return null;
        }
        public static Spritesheet TryFindSpriteSheet(string ResourceName)
        {
            if (spritesheets.ContainsKey(ResourceName))
            {
                return spritesheets[ResourceName];
            }
            return null;
        }

        public static Spritesheet getSpriteSheet(string path, string resourceName, float spriteWidth, float spriteHeight, int noSprites, int spacing)
        {
            // var root=Path.GetPathRoot(resourceName);
            if (spritesheets.ContainsKey(resourceName))
            {
                return spritesheets[resourceName];
            }
            Spritesheet sprite = new Spritesheet(AssetPool.getTexture(path, resourceName), resourceName, spriteWidth, spriteHeight, noSprites, spacing);
            AssetPool.spritesheets.Add(resourceName, sprite);
            return sprite;
        }

        private static void LoadAnimations(String fileName)
        {
            string serializedInfo = null;
            try
            {
                serializedInfo = File.ReadAllText("Assets.json");
                if (serializedInfo != "")
                {
                    Console.WriteLine(serializedInfo);

                    List<Animation> animations = JsonConvert.DeserializeObject<List<Animation>>(serializedInfo, new AssetDeserializer());
                    if (animations.Count > 0)
                    {
                        allAnimations = animations;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public static void LoadResources()
        {
            string folderPath = "Assets/Animations";
            if (Directory.Exists(folderPath))
            {
                // Get only JSON files in the folder
                string[] jsonFiles = Directory.GetFiles(folderPath, "*.json");

                // Display the list of JSON files
                Console.WriteLine("JSON files in the folder:");
                foreach (string jsonFile in jsonFiles)
                {
                    Console.WriteLine(jsonFile);
                    LoadAnimations(folderPath + "/" + jsonFile);
                }
            }
            else
            {
                Console.WriteLine("Folder does not exist: " + folderPath);
            }
        }

        public static void SaveResources()
        {
            string serializedInfo = null;
            serializedInfo = JsonConvert.SerializeObject(allAnimations);
            // Write JSON to file
            File.WriteAllText("Assets.json", serializedInfo);
        }

    }
}
