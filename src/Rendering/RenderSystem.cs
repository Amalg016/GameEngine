using GameEngine.scenes;
using GameEngine.Core.Utilities;
using Silk.NET.OpenGL;

namespace GameEngine.Rendering
{
    public class RenderSystem
    {
        private static FrameBuffer _frameBuffer;
        private static PickingTexture _pickingTexture;
        private Shader _defaultShader;
        private Shader _pickingShader;

        public static FrameBuffer FrameBuffer => _frameBuffer;
        public static PickingTexture PickingTexture => _pickingTexture;
        private GL gl;
        public void Initialize(GL gl, int width, int height)
        {
            this.gl = gl;
            gl.ClearColor(1, 1, 1, 1);
            gl.Enable(GLEnum.Blend);
            gl.BlendFunc(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha);

            _frameBuffer = new FrameBuffer(gl, (uint)width, (uint)height);
            _pickingTexture = new PickingTexture(gl, (uint)width, (uint)height);

            // Initialize shaders...
            _defaultShader = AssetPool.getShader("Assets/Shader/shader.vert", "Assets/Shader/shader.frag", "DefaultShader");
            _pickingShader = AssetPool.getShader("Assets/Shader/pickingShader.vert", "Assets/Shader/pickingShader.frag", "pickingShader");
        }

        public void Resize(int width, int height)
        {
            _frameBuffer?.Resize((uint)width, (uint)height);
            _pickingTexture?.Resize((uint)width, (uint)height);
        }

        public void RenderFrame(Scene currentScene, bool runtimePlaying)
        {
            // Picking phase
            gl.Disable(GLEnum.Blend);
            _pickingTexture.enableWriting();
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderPickingPhase(currentScene);
            _pickingTexture.disableWriting();

            // Main rendering phase

            gl.Enable(GLEnum.Blend);
            DebugDraw.beginFrame();
            _frameBuffer.Bind();
            gl.ClearColor(1, 1, 1, 1);
            // gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            gl.Clear(ClearBufferMask.ColorBufferBit);
            RenderMainPhase(currentScene, runtimePlaying);
            _frameBuffer.UnBind();
        }

        private void RenderMainPhase(Scene currentScene, bool runtimePlaying)
        {
            if (currentScene != null)
            {
                if (!runtimePlaying)
                {
                    DebugDraw.Draw();
                }
                Renderer.bindShader(_defaultShader);
                currentScene.Render();
            }
        }

        public void OnExit()
        {
            DebugDraw.OnExit();
            gl?.Dispose();
        }

        private void RenderPickingPhase(Scene currentScene)
        {
            Renderer.bindShader(_pickingShader);
            currentScene?.Render();
        }
    }
}
