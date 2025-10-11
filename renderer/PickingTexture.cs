using Silk.NET.OpenGL;

namespace GameEngine.renderer
{
    public class PickingTexture : IDisposable
    {
        public uint pickingTextureID;
        public uint fbo;
        private uint depthTexture;
        private GL gL = Window.gl;
        private uint width, height;
        private bool initialized = false;

        public PickingTexture(uint width, uint height)
        {
            this.width = width;
            this.height = height;
            if (!Initialize(width, height))
            {
                throw new Exception("Failed to initialize PickingTexture");
            }
        }

        private unsafe bool Initialize(uint width, uint height)
        {
            this.width = width;
            this.height = height;

            // Generate FBO
            fbo = gL.GenFramebuffer();
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            // Create picking texture
            pickingTextureID = gL.GenTexture();
            gL.BindTexture(TextureTarget.Texture2D, pickingTextureID);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            gL.TexImage2D(TextureTarget.Texture2D, 0, (int)GLEnum.Rgb32f, width, height, 0, GLEnum.Rgb, GLEnum.Float, null);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, pickingTextureID, 0);

            // Create depth texture
            depthTexture = gL.GenTexture();
            gL.BindTexture(TextureTarget.Texture2D, depthTexture);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            gL.TexImage2D(TextureTarget.Texture2D, 0, (int)GLEnum.DepthComponent, width, height, 0, GLEnum.DepthComponent, GLEnum.Float, null);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Texture2D, depthTexture, 0);

            // Set draw buffers
            gL.ReadBuffer(GLEnum.None);
            gL.DrawBuffer(GLEnum.ColorAttachment0);

            // Check framebuffer completeness
            if (gL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                Cleanup();
                return false;
            }

            // Unbind
            gL.BindTexture(GLEnum.Texture2D, 0);
            gL.BindFramebuffer(GLEnum.Framebuffer, 0);

            initialized = true;
            return true;
        }

        public void Resize(uint newWidth, uint newHeight)
        {
            if (newWidth == width && newHeight == height)
                return;

            Console.WriteLine($"Resizing PickingTexture from {width}x{height} to {newWidth}x{newHeight}");

            // Clean up old resources
            Cleanup();

            // Reinitialize with new dimensions
            if (!Initialize(newWidth, newHeight))
            {
                throw new Exception("Failed to resize PickingTexture");
            }
        }

        private void Cleanup()
        {
            if (pickingTextureID != 0)
            {
                gL.DeleteTexture(pickingTextureID);
                pickingTextureID = 0;
            }

            if (depthTexture != 0)
            {
                gL.DeleteTexture(depthTexture);
                depthTexture = 0;
            }

            if (fbo != 0)
            {
                gL.DeleteFramebuffer(fbo);
                fbo = 0;
            }

            initialized = false;
        }

        public void enableWriting()
        {
            if (!initialized) return;
            gL.BindFramebuffer(GLEnum.DrawFramebuffer, fbo);
        }

        public void disableWriting()
        {
            gL.BindFramebuffer(GLEnum.DrawFramebuffer, 0);
        }

        public unsafe int readPixel(int x, int y)
        {
            if (!initialized) return -1;

            // Flip Y coordinate (OpenGL has origin at bottom-left)
            int flippedY = (int)height - y - 1;

            // Clamp coordinates to valid range
            if (x < 0 || x >= width || flippedY < 0 || flippedY >= height)
                return -1;

            gL.BindFramebuffer(GLEnum.ReadFramebuffer, fbo);
            gL.ReadBuffer(GLEnum.ColorAttachment0);

            float[] pixels = new float[3];
            fixed (void* d = pixels)
            {
                gL.ReadPixels(x, flippedY, 1, 1, GLEnum.Rgb, GLEnum.Float, d);
            }

            gL.BindFramebuffer(GLEnum.ReadFramebuffer, 0);

            return (int)(pixels[0]) - 1;
        }

        public void Dispose()
        {
            Cleanup();
        }

        public uint GetWidth() => width;
        public uint GetHeight() => height;
        public bool IsInitialized() => initialized;
    }
}
