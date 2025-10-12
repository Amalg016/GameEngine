using GameEngine.components;
using Silk.NET.OpenGL;

namespace GameEngine.Rendering.Core
{
    public class Renderer
    {
        int MaxBatchSize = 1000;
        public List<RenderBatch> batches = new List<RenderBatch>();
        GL GL;
        private static Shader shader;

        public Renderer(GL gl)
        {
            GL = gl;
        }

        public void Add(GameObject g)
        {
            SpriteRenderer spr = g.GetComponent<SpriteRenderer>();
            if (spr != null)
            {
                Add(spr);
            }
        }

        void Add(SpriteRenderer sprite)
        {
            bool added = false;

            foreach (RenderBatch batch in batches)
            {
                if (batch.HasRoom() && batch.zIndex() == sprite.gameObject.zIndex())
                {
                    Texture tex = sprite.GetTexture();
                    if (tex == null && (batch.hasTexture(tex) || batch.hasTextureRoom()))
                    {
                        batch.AddSprite(sprite);
                        added = true;
                        break;
                    }
                }
            }
            if (!added)
            {
                RenderBatch newBatch = new RenderBatch(GL, MaxBatchSize, sprite.gameObject.zIndex(), this);
                newBatch.Start();
                batches.Add(newBatch);
                newBatch.AddSprite(sprite);
                batches.Sort((a, b) => a.zIndex().CompareTo(b.zIndex()));
                sprite.setDirty();
            }
        }



        public static void bindShader(Shader newShader)
        {
            shader = newShader;
        }

        public static Shader getBoundShader()
        {
            return shader;
        }
        public void render()
        {
            shader.Use();
            for (int i = 0; i < batches.Count; i++)
            {
                RenderBatch batch = batches[i];
                batch.render();

            }
            foreach (RenderBatch batch in batches)
            {
            }
        }

        public void DestroyGameObject(GameObject go)
        {
            if (go.GetComponent<SpriteRenderer>() == null) return;
            foreach (var item in batches)
            {
                if (item.destroyIfExists(go))
                {
                    return;
                }
            }
        }
    }
}
