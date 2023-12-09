using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace GameEngine.renderer
{
    public class Texture : IDisposable
    {
        private uint textureID;
        private GL _gl { get { return Window.gl; } }
        private int width;
        private int height;
        //    [JsonRequired]string path;
        // [JsonConstructor()]
        public unsafe Texture(string path)
        {
            //  _gl = gl;
            this.path = path;

            textureID = _gl.GenTexture();
            Bind();

            //Loading an image using imagesharp.
            using (var img = Image.Load<Rgba32>(path))
            {
                width = img.Width;
                height = img.Height;
                //Reserve enough memory from the gpu for the whole image
                _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, (uint)img.Width, (uint)img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, null);

                img.ProcessPixelRows(accessor =>
                {
                    //ImageSharp 2 does not store images in contiguous memory by default, so we must send the image row by row
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        fixed (void* data = accessor.GetRowSpan(y))
                        {
                            //Loading the actual image.
                            _gl.TexSubImage2D(TextureTarget.Texture2D, 0, 0, y, (uint)accessor.Width, 1, PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    }
                });
            }

            SetParameters();

        }

        public Texture()
        {
            throw new Exception("null texture:filepath not specified");
        }

        public unsafe Texture(uint width, uint height)
        {
            this.path = "Generated";
            // this.width = (int)width;
            //this.height =(int) height;
            textureID = _gl.GenTexture();
            _gl.BindTexture(GLEnum.Texture2D, textureID);

              _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMinFilter, (int)GLEnum.Linear);
              _gl.TexParameter(GLEnum.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
          //  _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
          //  _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);

            _gl.TexImage2D(GLEnum.Texture2D, 0, InternalFormat.Rgb, width , height, 0, GLEnum.Rgb, GLEnum.UnsignedByte, null);

        }
        public int getWidth()
        {
            return width;
        }
        public uint getTexID()
        {
            return textureID;
        }
        public int getHeight()
        {
            return height;
        }
        string path;
        public string getFilePath()
        {
            return path;
        }
        public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
        {
            //Saving the gl instance.
            //_gl = gl;
            //Generating the opengl handle;
            textureID = _gl.GenTexture();
            Bind();

            //We want the ability to create a texture using data generated from code aswell.
            fixed (void* d = &data[0])
            {
                //Setting the data of a texture.
                _gl.TexImage2D(TextureTarget.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, d);
                SetParameters();
            }
        }

        private void SetParameters()
        {
            //Setting some texture perameters so the texture behaves as expected.
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.LinearMipmapLinear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBaseLevel, 0);
            _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, 8);
            //Generating mipmaps.
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        public void Bind()
        {
            _gl.BindTexture(TextureTarget.Texture2D, textureID);
        }
        public void UnBind()
        {
            _gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind(int i)
        {
            //When we bind a texture we can choose which textureslot we can bind it to.
            _gl.ActiveTexture(TextureUnit.Texture0 + i);
            _gl.BindTexture(TextureTarget.Texture2D, textureID);
        }

        public void Dispose()
        {
            //In order to dispose we need to delete the opengl handle for the texure.
            _gl.DeleteTexture(textureID);
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj.GetType() == typeof(Texture))) return false;
            Texture other = (Texture)obj;
            return other.getWidth() == this.getWidth() && other.getHeight() == this.getHeight() && other.getTexID() == this.textureID && other.getFilePath().Equals(this.path);
            //   return true;
        }
    }
}