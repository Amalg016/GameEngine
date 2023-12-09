using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.renderer
{
    public class FrameBuffer
    {
        uint fboID;
        GL gL = Window.gl;
        private Texture texture;
        public  FrameBuffer(uint width,uint height)
        {
            fboID = gL.GenFramebuffer();
            gL.BindFramebuffer(FramebufferTarget.Framebuffer, fboID);

            //create texture
            this.texture = new Texture(width, height);
            gL.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D,(uint) this.texture.getTexID(),0);

            //
            uint rboID = gL.GenRenderbuffer();
            gL.BindRenderbuffer(GLEnum.Renderbuffer, fboID);
            gL.RenderbufferStorage(GLEnum.Renderbuffer,GLEnum.DepthComponent32,width,height);
            gL.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthAttachment, GLEnum.Renderbuffer, rboID);

            if (gL.CheckFramebufferStatus(GLEnum.Framebuffer) != GLEnum.FramebufferComplete)
            {
                throw new Exception("Error: FrameBuffer is not Complete");
            }
            gL.BindFramebuffer(GLEnum.Framebuffer, 0);
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
            gL.Dispose();
        }
        public uint getFboID()
        {
            return fboID;
        }
        public uint getTextureID()
        {
            return texture.getTexID();
        }
       
    }
}