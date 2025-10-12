using Silk.NET.OpenGL;
using System;

namespace GameEngine.renderer
{
    public class FrameBuffer : IDisposable
    {
        private uint fboID;
        private uint rboID;
        private GL gL = Window.gl;
        private Texture texture;
        private uint width, height;

        public FrameBuffer(uint width, uint height)
        {
            this.width = width;
            this.height = height;
            Initialize(width, height);
        }
        public FrameBuffer(GL gl, uint width, uint height)
        {
            this.width = width;
            this.height = height;
            this.gL = gl;
            Initialize(width, height);
        }

        private void Initialize(uint width, uint height)
        {
            // Generate FBO
            fboID = gL.GenFramebuffer();
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);

            // Create texture
            this.texture = new Texture(width, height);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D,
                                   (uint)this.texture.getTexID(), 0);

            // Generate RBO (Render Buffer Object) for depth
            rboID = gL.GenRenderbuffer();
            gL.BindRenderbuffer(GLEnum.Renderbuffer, rboID); // Bind RBO, not FBO!
            gL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent32, width, height);
            gL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment,
                                      GLEnum.Renderbuffer, rboID);

            // Check framebuffer completeness
            if (gL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                throw new Exception("Error: FrameBuffer is not Complete");
            }

            gL.BindFramebuffer(GLEnum.Framebuffer, 0);
            gL.BindRenderbuffer(GLEnum.Renderbuffer, 0);
        }

        public void Resize(uint newWidth, uint newHeight)
        {
            if (newWidth == width && newHeight == height)
                return;

            this.width = newWidth;
            this.height = newHeight;

            // Clean up old resources
            Cleanup();

            // Reinitialize with new dimensions
            Initialize(newWidth, newHeight);
        }

        private void Cleanup()
        {
            if (fboID != 0)
            {
                gL.DeleteFramebuffer(fboID);
                fboID = 0;
            }

            if (rboID != 0)
            {
                gL.DeleteRenderbuffer(rboID);
                rboID = 0;
            }

            // Dispose texture if it implements IDisposable
            if (texture is IDisposable disposableTexture)
            {
                disposableTexture.Dispose();
            }
            texture = null;
        }

        public void Bind()
        {
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);
        }

        public void UnBind()
        {
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            Cleanup();
        }

        public uint getFboID()
        {
            return fboID;
        }

        public uint getTextureID()
        {
            return texture?.getTexID() ?? 0;
        }

        public uint GetWidth() => width;
        public uint GetHeight() => height;
    }
}
