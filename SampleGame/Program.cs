using GameEngine.Core.Application;
using GameEngine.Core.Utilities;
using GameEngine.ECS;
using GameEngine.components;
using GameEngine.scenes;
using GameEngine.Serialization;
using GameEngine.observers.events;
using GameEngine.observers;
using System.Numerics;

namespace SampleGame
{
    /// <summary>
    /// Minimal game scene initializer — demonstrates building a game
    /// using only the engine library, without any editor dependency.
    /// </summary>
    public class GameSceneInitializer : sceneInitializer
    {
        public override void Init(Scene scene)
        {
            // Create a player with a sprite and the controller
            Spritesheet sheet = AssetPool.TryFindSpriteSheet("sheet1");
            GameObject player = new GameObject();
            player.Load("Player", 0);
            player.transform.position = new Vector2(0, 0);
            player.transform.scale = new Vector2(1f, 1f);

            SpriteRenderer renderer = new SpriteRenderer();
            renderer.init(sheet.GetSprite(0));
            player.AddComponent(renderer);
            player.AddComponent(new PlayerController());

            scene.addGameObjectToScene(player);
        }

        public override void loadResources(Scene scene)
        {
            AssetPool.getShader("Assets/Shader/shader.vert", "Assets/Shader/shader.frag", "DefaultShader");

            // Load your game assets (NOT editor assets like Editor/Images/gizmos.png)
            AssetPool.getSpriteSheet("Assets/Images/Scavengers_SpriteSheet.png", "sheet1", 32f, 32f, 55, 0);
            AssetPool.getTexture("Assets/Images/ezgif.com-gif-maker (1).png", "Player1");
            AssetPool.getTexture("Assets/Images/silk.png", "Player2");
        }

        public override void imgui()
        {
            // No ImGui in game builds
        }

        public override void Exit(Scene scene)
        {
            foreach (var item in scene.renderer.batches)
            {
                item.OnExit();
            }
        }
    }

    class GameCore : GameEngineCore
    {
        private static GameCore _gameInstance;
        public static new GameCore Instance => _gameInstance ??= new GameCore();

        protected GameCore() : base()
        {
            SceneManager.SceneInitializerFactory = () => new GameSceneInitializer();
            GameEngine.Rendering.Core.RenderSystem.UseFramebuffer = false;

            // Register all game components so they can be serialized/deserialized
            ComponentRegistry.RegisterAllFromAssembly(typeof(GameCore).Assembly);
        }

        protected override sceneInitializer CreateInitialScene()
        {
            return new GameSceneInitializer();
        }

        protected override void OnWindowLoad()
        {
            base.OnWindowLoad();
            GameEngine.observers.EventSystem.notify(null, new GameEngine.observers.events.Event(GameEngine.observers.events.EventType.GameEngineStartPlay));
        }
    }

    class Program
    {
        public static void Main(params string[] args)
        {
            GameCore engineCore = GameCore.Instance;
            engineCore.Start();
            // EventSystem.notify(null, new Event(EventType.GameEngineStartPlay));
        }
    }
}
