using Silk.NET.OpenGL;
using Silk.NET.Vulkan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.renderer
{
    public class PickingTexture
    {
        public uint pickingTextureID;
        public uint fbo;
        private uint depthTexture;
        GL gL = Window.gl;
        public PickingTexture(uint width,uint height)
        {
            if (!Init(width, height))
            {
                throw new ArgumentNullException("width or height not specified");
            }
        }
        public unsafe bool Init(uint width,uint height)
        {
        
            fbo = gL.GenFramebuffer();
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, fbo);

            //create textureID
            pickingTextureID = gL.GenTexture();
            gL.BindTexture(TextureTarget.Texture2D, pickingTextureID);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gL.TexParameter(TextureTarget.Texture2D,TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
            gL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            gL.TexImage2D(TextureTarget.Texture2D, 0,(int) GLEnum.Rgb32f,width,height, 0, GLEnum.Rgb, GLEnum.Float, null);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, (uint)this.pickingTextureID, 0);


            gL.Enable(GLEnum.Texture2D);
            depthTexture=gL.GenTexture();
            gL.BindTexture(TextureTarget.Texture2D,depthTexture);
            gL.TexImage2D(TextureTarget.Texture2D, 0,(int) GLEnum.DepthComponent, width, height, 0, GLEnum.DepthComponent, GLEnum.Float, null);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Texture2D, depthTexture, 0);


            gL.ReadBuffer(GLEnum.None);
            gL.DrawBuffer(GLEnum.ColorAttachment0);
            //
          //  uint rboID = gL.GenRenderbuffer();
          //  gL.BindRenderbuffer(GLEnum.Renderbuffer, fbo);
          //  gL.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.DepthComponent32, width, height);
          //  gL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, rboID);
          //
            if (gL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                return false;
                throw new Exception("Error: FrameBuffer is not Complete");
            }

            gL.BindTexture(GLEnum.Texture2D, 0);
            gL.BindFramebuffer(GLEnum.Framebuffer, 0);
            return true;
        }

        public void enableWriting()
        {
            gL.BindFramebuffer(GLEnum.DrawFramebuffer, fbo);
        }
        public void disableWriting()
        {
            gL.BindFramebuffer(GLEnum.DrawFramebuffer, 0);
        }
        public unsafe int readPixel(int x,int y)
        {
            gL.BindFramebuffer(GLEnum.ReadFramebuffer, fbo);
            gL.ReadBuffer(GLEnum.ColorAttachment0);

            float[] pixels=new float[3];
            fixed (void* d = pixels)
            {
                gL.ReadPixels(x, y, 1, 1, GLEnum.Rgb, GLEnum.Float, d);
            }
            return (int)(pixels[0])-1;
        }
    }
}
