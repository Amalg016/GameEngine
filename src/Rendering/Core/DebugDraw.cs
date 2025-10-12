using GameEngine.Core.Math;
using GameEngine.Core.Utilities;
using Silk.NET.OpenGL;
using System.Numerics;

namespace GameEngine.Rendering.Core
{
    public class DebugDraw
    {
        private static int Max_Lines = 5000;

        private static List<Line2D> Lines = new List<Line2D>();

        private static float[] vertexArray = new float[Max_Lines * 6 * 2];
        private static Shader shader = AssetPool.getShader("Assets/Shader/DebugLine.vert", "Assets/Shader/DebugLine.frag", "DebugDrawShader");

        public static BufferObject<float> Vbo;
        public static VertexArrayObject<float, uint> Vao;

        private static bool started = false;
        public static GL gL = Window.gl;
        public static void start()
        {
            Vbo = new BufferObject<float>(gL, vertexArray, BufferTargetARB.ArrayBuffer);
            Vao = new VertexArrayObject<float, uint>(gL, Vbo, null);

            Vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 6, 0);
            Vao.VertexAttributePointer(1, 3, VertexAttribPointerType.Float, 6, 3);

            // Set Line Width
            gL.LineWidth(5);
        }

        public static void beginFrame()
        {
            if (!started)
            {
                start();
                started = true;
            }
            //Remove Dead Line
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].beginFrame() <= 0)
                {
                    Lines.Remove(Lines[i]);
                    i--;
                }
            }

        }
        public static void Draw()
        {
            if (Lines.Count == 0) return;
            int index = 0;
            foreach (var item in Lines)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position;
                    if (i == 0)
                    {

                        position = item.getFrom();
                    }
                    else
                    {
                        position = item.getto();
                    }
                    Vector3 color = item.getColor();

                    //Loading position
                    vertexArray[index] = position.X;
                    vertexArray[index + 1] = position.Y;
                    vertexArray[index + 2] = -10;

                    //Loading color
                    vertexArray[index + 3] = color.X;
                    vertexArray[index + 4] = color.Y;
                    vertexArray[index + 5] = color.Z;

                    index += 6;
                }
            }
            Vbo.Bind();
            //float[] g=new float[];
            //Console.WriteLine(g[0]);
            float[] g = vertexArray;
            Array.Resize(ref g, Lines.Count * 6 * 2);
            //  Vao.Bind();
            Vbo.BindBuffer2(g);
            // Vbo.BindBuffer(vertexArray);
            shader.Use();

            shader.SetUniform("uProjection", Window.camera.GetProjectionMatrix());
            shader.SetUniform("uView", Window.camera.getViewMatrix());

            Vao.Bind();
            gL.EnableVertexAttribArray(0);
            gL.EnableVertexAttribArray(1);

            gL.DrawArrays(PrimitiveType.Lines, 0, (uint)Lines.Count * 6 * 2);


            gL.DisableVertexAttribArray(0);
            gL.DisableVertexAttribArray(1);
            Vao.UnBind();
            Vbo.UnBind();
            shader.detach();

            //   for (int i = 0; i < vertexArray.Length; i++)
            //   {
            //       vertexArray[i] = 0;
            //   }
        }

        public static void addLine2D(Vector2 from, Vector2 to)
        {
            DebugDraw.addLine2D(from, to, new Vector3(0, 0, 0), 1);
        }
        public static void addLine2D(Vector2 from, Vector2 to, Vector3 color)
        {
            DebugDraw.addLine2D(from, to, color, 2);
        }
        public static void addLine2D(Vector2 from, Vector2 to, Vector3 color, int lifetime)
        {
            if (Lines.Count >= Max_Lines) return;
            DebugDraw.Lines.Add(new Line2D(from, to, color, lifetime));
        }
        public static void addBox2D(Vector2 center, Vector2 dimensions, float Rotation)
        {
            addBox2D(center, dimensions, Rotation, new Vector3(0, 1, 0), 2);
        }
        public static void addBox2D(Vector2 center, Vector2 dimensions, float Rotation, Vector3 color, int lifetime)
        {
            Vector2 min = center - (dimensions * 0.5f);
            Vector2 max = center + (dimensions * 0.5f);

            Vector2[] vertices =
            {
                new Vector2(min.X,min.Y),new Vector2(min.X,max.Y),
                new Vector2(max.X,max.Y),new Vector2(max.X,min.Y)
            };

            if (Rotation != 0)
            {

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] = Mathf.RotateAboutOrigin(vertices[i], center, Rotation);
                }

            }
            addLine2D(vertices[0], vertices[1], color, lifetime);
            addLine2D(vertices[0], vertices[3], color, lifetime);
            addLine2D(vertices[1], vertices[2], color, lifetime);
            addLine2D(vertices[2], vertices[3], color, lifetime);
        }
        public static void addCircle2D(Vector2 center, float Radius, Vector3 color, int lifetime)
        {
            Vector2[] points = new Vector2[20];
            int increment = 360 / points.Length;
            int currentAngle = 0;

            for (int i = 0; i < points.Length; i++)
            {
                Vector2 tmp = new Vector2(0, Radius);
                tmp = Mathf.RotateAboutOrigin(tmp, new Vector2(), currentAngle);
                points[i] = tmp + center;
                if (i > 0)
                {
                    addLine2D(points[i - 1], points[i], color, lifetime);
                }
                currentAngle += increment;
                //    Console.WriteLine(points[i].X + "Y" + points[i].Y);
            }
            addLine2D(points[points.Length - 1], points[0], color, lifetime);
        }

        public static void OnExit()
        {
            shader.Dispose();
            Vbo.Dispose();
            Vao.Dispose();
        }
    }
}
