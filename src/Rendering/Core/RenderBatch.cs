using GameEngine.components;
using GameEngine.Core.Math;
using Silk.NET.OpenGL;
using System.Numerics;

namespace GameEngine.Rendering.Core
{
    public class RenderBatch
    {
        private const int Pos_Size = 3;
        private const int Color_Size = 4;
        private const int Tex_Coords_Size = 2;
        private const int Tex_ID_Size = 1;
        private const int Entity_ID_Size = 1;
        private const int Pos_Offset = 0;
        private int Color_Offset { get { return Pos_Offset + Pos_Size; } }
        private int Tex_coords_Offset { get { return Color_Offset + Color_Size; } }
        private int Tex_ID_Offset { get { return Tex_coords_Offset + Tex_Coords_Size; } }
        private int Entity_ID_Offset { get { return Tex_ID_Offset + Tex_ID_Size; } }

        private const uint Vertex_Size = 11;

        private List<Texture> textures = new List<Texture>();
        private SpriteRenderer[] sprites;
        private int numSprites;
        private bool hasRoom;
        private float[] vertices;
        private int[] texSlots = { 0, 1, 2, 3, 4, 5, 6, 7 };
        private int maxBatchSize;
        public Shader shader;
        GL GL;

        public BufferObject<float> Vbo;
        public BufferObject<uint> Ebo;
        public VertexArrayObject<float, uint> Vao;

        int ZIndex;
        Renderer renderer;
        public RenderBatch(GL gl, int maxBatch, int Zind, Renderer renderer)
        {
            this.renderer = renderer;
            this.sprites = new SpriteRenderer[maxBatch];
            this.maxBatchSize = maxBatch;
            this.ZIndex = Zind;
            vertices = new float[maxBatchSize * 4 * Vertex_Size];

            this.numSprites = 0;
            this.hasRoom = true;
            this.GL = gl;
            //  shader = new Shader(GL, "Assets/Shader/shader.vert", "Assets/Shader/shader.frag");
        }

        public void AddSprite(SpriteRenderer spr)
        {
            int index = this.numSprites;

            sprites[index] = spr;
            numSprites++;

            if (spr.GetTexture() != null)
            {
                if (!textures.Contains(spr.GetTexture()))
                {
                    textures.Add(spr.GetTexture());
                }
            }

            loadVertexProperties(index);

            if (numSprites >= maxBatchSize)
            {
                hasRoom = false;
            }

        }
        // public void RemoveVertexProperties(int index)
        // {
        //
        // }
        private void loadVertexProperties(int index)
        {
            SpriteRenderer sprite = this.sprites[index];

            int offset = index * 4 * (int)Vertex_Size;
            Vector4 color = sprite.GetColor();
            Vector2[] texCoords = sprite.GetTexCoords();


            int texID = 0;
            if (sprite.GetTexture() != null)
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    if (textures[i].Equals(sprite.GetTexture()))
                    {
                        texID = i + 1;
                        break;
                    }
                }
            }

            bool isRotated = sprite.gameObject.transform.rotation != 0;
            Matrix4x4 transformMatrix = Matrix4x4.Identity;
            if (isRotated)
            {
                Mathf.translate(new Vector3(sprite.gameObject.transform.position.X, sprite.gameObject.transform.position.Y, 0), transformMatrix, ref transformMatrix);
                // transformMatrix.Translation =new Vector3( sprite.gameObject.transform.position.X, sprite.gameObject.transform.position.Y,0);
                Mathf.Rotate((float)Mathf.DegreesToRadians(sprite.gameObject.transform.rotation), new Vector3(0, 0, 1), transformMatrix, ref transformMatrix);
                Mathf.scale(new Vector3(sprite.gameObject.transform.scale.X, sprite.gameObject.transform.scale.Y, 1), transformMatrix, ref transformMatrix);
            }


            float xAdd = 0.5f;
            float yAdd = 0.5f;
            for (int i = 0; i < 4; i++)
            {

                if (i == 1)
                {
                    yAdd = -0.5f;
                }
                else if (i == 2)
                {
                    xAdd = -0.5f;
                }
                else if (i == 3)
                {
                    yAdd = 0.5f;
                }

                Vector4 currentPos = new Vector4(sprite.gameObject.transform.position.X + (xAdd * sprite.gameObject.transform.scale.X), sprite.gameObject.transform.position.Y + (yAdd * sprite.gameObject.transform.scale.Y), 0, 1);
                if (isRotated)
                {
                    // currentPos = new Vector4(xAdd, yAdd, 0, 1) * transformMatrix;
                    currentPos = Mathf.Multiply(transformMatrix, new Vector4(xAdd, yAdd, 0, 1));
                }

                vertices[offset] = currentPos.X;
                vertices[offset + 1] = currentPos.Y;
                vertices[offset + 2] = 0;

                vertices[offset + 3] = color.X;
                vertices[offset + 4] = color.Y;
                vertices[offset + 5] = color.Z;
                vertices[offset + 6] = color.W;

                vertices[offset + 7] = texCoords[i].X;
                vertices[offset + 8] = texCoords[i].Y;

                vertices[offset + 9] = texID;

                //Load entity id
                vertices[offset + 10] = sprite.gameObject.getUid() + 1;

                offset += (int)Vertex_Size;
            }
        }
        uint[] indices;
        public void Start()
        {
            indices = generateIndices();

            Ebo = new BufferObject<uint>(GL, indices, BufferTargetARB.ElementArrayBuffer);
            Vbo = new BufferObject<float>(GL, vertices, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(GL, Vbo, Ebo);

            Vao.VertexAttributePointer(0, Pos_Size, VertexAttribPointerType.Float, Vertex_Size, Pos_Offset);
            // GL.EnableVertexAttribArray(0);

            Vao.VertexAttributePointer(1, Color_Size, VertexAttribPointerType.Float, Vertex_Size, Color_Offset);
            Vao.VertexAttributePointer(2, Tex_Coords_Size, VertexAttribPointerType.Float, Vertex_Size, Tex_coords_Offset);
            Vao.VertexAttributePointer(3, Tex_ID_Size, VertexAttribPointerType.Float, Vertex_Size, Tex_ID_Offset);
            Vao.VertexAttributePointer(4, Entity_ID_Size, VertexAttribPointerType.Float, Vertex_Size, Entity_ID_Offset);
            // GL.EnableVertexAttribArray(1);

            //      GL.VertexAttribPointer(2, Tex_Coords_Size, GLEnum.Float, false, Vertex_Size_Bytes,(void*) Tex_coords_Offset);
            //      GL.EnableVertexAttribArray(2);
            //     
            //      GL.VertexAttribPointer(3, Tex_ID_Size, GLEnum.Float, false, Vertex_Size_Bytes,(void*) Tex_ID_Offset);
            //      GL.EnableVertexAttribArray(3);


        }


        public unsafe void render()
        {
            bool reBufferData = false;
            for (int i = 0; i < numSprites; i++)
            {
                SpriteRenderer spr = sprites[i];

                if (spr.isDirty())
                {
                    loadVertexProperties(i);
                    spr.setClean();
                    reBufferData = true;
                }


                if (spr.gameObject.zIndex() != this.ZIndex)
                {
                    destroyIfExists(spr.gameObject);
                    renderer.Add(spr.gameObject);
                    i--;
                }

            }
            if (reBufferData)
            {
                Ebo.Bind();
                Vbo.Bind();
                Vbo.BindBuffer(vertices);
            }
            Vao.Bind();
            // Console.WriteLine(BatchNo);

            //  shader.Use();
            shader = Renderer.getBoundShader();
            shader.SetUniform("uProjection", Window.camera.GetProjectionMatrix());
            shader.SetUniform("uView", Window.camera.getViewMatrix());
            shader.SetUniform("uTextures", texSlots);

            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].Bind(i + 1);
            }

            Vao.Bind();

            GL.DrawElements(PrimitiveType.Triangles, (uint)numSprites * 6, DrawElementsType.UnsignedInt, null);


            for (int i = 0; i < textures.Count; i++)
            {
                textures[i].UnBind();
            }


            Vao.UnBind();
            Ebo.UnBind();
            Vbo.UnBind();
            shader.detach();
        }

        private uint[] generateIndices()
        {


            uint[] elements = new uint[6 * maxBatchSize];
            for (int i = 0; i < maxBatchSize; i++)
            {
                loadElementIndices(elements, i);
            }
            return elements;
        }

        private void loadElementIndices(uint[] elements, int index)
        {
            int offsetArrayIndex = 6 * index;
            int offset = 4 * index;

            elements[offsetArrayIndex] = (uint)offset + 0;
            elements[offsetArrayIndex + 1] = (uint)offset + 1;
            elements[offsetArrayIndex + 2] = (uint)offset + 3;

            elements[offsetArrayIndex + 3] = (uint)offset + 3;
            elements[offsetArrayIndex + 4] = (uint)offset + 2;
            elements[offsetArrayIndex + 5] = (uint)offset + 1;
        }

        public void OnExit()
        {
            Vbo.Dispose();
            Ebo.Dispose();
            Vao.Dispose();
            shader.Dispose();
            foreach (var item in textures)
            {
                item.Dispose();
            }
        }

        public bool HasRoom()
        {
            return this.hasRoom;
        }
        public bool hasTextureRoom()
        {
            return textures.Count < 8;
        }
        public bool hasTexture(Texture texture)
        {
            return textures.Contains(texture);
        }

        public int zIndex()
        {
            return this.ZIndex;
        }

        public bool destroyIfExists(GameObject go)
        {
            SpriteRenderer sprite = go.GetComponent<SpriteRenderer>();
            for (int i = 0; i < numSprites; i++)
            {
                if (sprites[i] == sprite)
                {
                    for (int j = i; j < numSprites - 1; j++)
                    {
                        sprites[j] = sprites[j + 1];
                        sprites[j].setDirty();
                    }
                    numSprites--;
                    return true;
                }
            }
            return false;
        }
    }
}
