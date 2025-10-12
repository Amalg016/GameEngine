using System.Numerics;
using GameEngine.components;
using GameEngine.Core.Utilities;
using ImGuiNET;
using Shader = GameEngine.Rendering.Shader;
using Texture = GameEngine.Rendering.Texture;



namespace GameEngine.scenes
{
    public class LevelEditorSceneInitializer : sceneInitializer
    {

        private const int Width = 800;
        private const int Height = 700;
        static Texture texture;
        Shader shader;

        private BufferObject<float> Vbo;
        private BufferObject<uint> Ebo;
        private VertexArrayObject<float, uint> Vao;

        GameObject go2;
        Spritesheet spritesheet;

        GameObject LevelEditorStuff = new GameObject();

        public override void Init(Scene scene)
        {

            LevelEditorStuff.Load("LevelEditor", 1);
            LevelEditorStuff.AddComponent(new MouseControls());
            LevelEditorStuff.AddComponent(new GridLines());
            LevelEditorStuff.AddComponent(new TranslateGizmo(gizmos.GetSprite(1), Window.GetGUISystem().GetPropertiesWindow()));
            LevelEditorStuff.AddComponent(new ScaleGizmo(gizmos.GetSprite(2), Window.GetGUISystem().GetPropertiesWindow()));
            LevelEditorStuff.AddComponent(new NonPickable());
            LevelEditorStuff.dontSerialize();
            LevelEditorStuff.AddComponent(new GizmoSystem(gizmos));
            scene.addGameObjectToScene(LevelEditorStuff);

        }
        Spritesheet gizmos;
        public override void loadResources(Scene scene)
        {

            AssetPool.getShader("Assets/Shader/shader.vert", "Assets/Shader/shader.frag", "DefaultShader");
            AssetPool.getSpriteSheet("Editor/Images/gizmos.png", "Arrows", 24, 48, 3, 0);
            //  spritesheet = new Spritesheet(AssetPool.getTexture("tileset.png", "Lvlsheet"),16f,16,240,0);
            spritesheet = AssetPool.getSpriteSheet("Assets/Images/Scavengers_SpriteSheet.png", "sheet1", 32f, 32f, 55, 0);
            AssetPool.getTexture("Assets/Images/ezgif.com-gif-maker (1).png", "Player1");

            AssetPool.getTexture("Assets/Images/silk.png", "Player2");

            gizmos = AssetPool.TryFindSpriteSheet("Arrows");

            spritesheet = AssetPool.TryFindSpriteSheet("sheet1");
        }

        private int spriteIndex = 0;
        float spriteFlipTime = .2f;
        float spriteFlipTimeLeft = 0;






        private static readonly float[] Vertices =
        {
            //X    Y      Z        R  G  B  A    u v
             0.5f,  0.5f, 0.0f,    1, 1, 1, 1,   1,0,
             0.5f, -0.5f, 0.0f,    1, 1, 1, 1,   1,1,
            -0.5f, -0.5f, 0.0f,    1, 1, 1, 1,   0,1,
            -0.5f,  0.5f, 0.0f,    1, 1, 1, 1,   0,0
        };


        private static readonly uint[] Indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        //   public unsafe void Render()
        //   {
        //     gl.Clear(ClearBufferMask.ColorBufferBit);
        //     Vao.Bind();
        //
        //     shader.Use();
        //   //  texture.Bind();
        //     shader.SetUniform("uProjection", Camera.GetProjectionMatrix());
        //     shader.SetUniform("uView", Camera.GetViewMatrix());
        //   //  shader.SetUniform("uTexture0", 0);
        //   
        //     gl.DrawElements(PrimitiveType.Triangles, (uint)Indices.Length, DrawElementsType.UnsignedInt, null);
        //   }

        public override void imgui()
        {
            ImGui.Begin("LevelEditorStuff");
            LevelEditorStuff.IMGUI();
            ImGui.End();

            ImGui.Begin("Stuffs");
            if (ImGui.BeginTabBar("Window TabBar"))
            {
                if (ImGui.BeginTabItem("Sprites"))
                {
                    Vector2 windowPos = ImGui.GetWindowPos();
                    Vector2 windowSize = ImGui.GetWindowSize();
                    Vector2 itemSpacing = ImGui.GetStyle().ItemSpacing;
                    float windowX2 = windowPos.X + windowSize.X;
                    for (int i = 0; i < spritesheet.size(); i++)
                    {
                        Sprite sprite = spritesheet.GetSprite(i);
                        float spriteWidth = sprite.GetWidth();
                        float spriteHeight = sprite.GetHeight();
                        int id = sprite.GetTexID();
                        Vector2[] texCoords = sprite.getTexCoords();

                        Vector2 spriteSize = new Vector2(spriteWidth, spriteHeight);
                        Vector2 uv0 = new Vector2(texCoords[1].X, texCoords[1].Y);
                        Vector2 uv1 = new Vector2(texCoords[3].X, texCoords[3].Y);
                        ImGui.PushID(i);
                        if (ImGui.ImageButton(uv1.ToString() + uv0.ToString(), (IntPtr)id, spriteSize, uv1, uv0))
                        {
                            GameObject block = Prefab.generateSpriteObject(spritesheet.GetSprite(i), 0.25f, 0.25f, 0);
                            block.dontSerialize();

                            LevelEditorStuff.GetComponent<MouseControls>().pickUpObject(block);
                            //    block.AddComponent(new Rigidbody2D());
                        }
                        ImGui.PopID();

                        //   Console.WriteLine("go2" + sceneGameObjects[6].transform.position.ToString());
                        Vector2 lastButtonPos = ImGui.GetItemRectMax();
                        float lastButtonX2 = lastButtonPos.X;
                        float nextButtonX2 = lastButtonX2 + itemSpacing.X + spriteWidth;
                        if (i + 1 < spritesheet.size() && nextButtonX2 < windowX2)
                        {
                            ImGui.SameLine();
                        }
                    }
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Prefabs"))
                {



                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
            ImGui.End();
        }

        public override void Exit(Scene scene)
        {
            foreach (var item in scene.renderer.batches)
            {
                item.OnExit();
            }
            //  Console.WriteLine(scene.sceneGameObjects.Count);
        }


    }
}
