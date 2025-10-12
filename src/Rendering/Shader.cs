using System.Numerics;
using Silk.NET.OpenGL;

namespace GameEngine.Rendering
{
    public class Shader : IDisposable
    {

        //Our handle and the GL instance this class will use, these are private because they have no reason to be public.
        //Most of the time you would want to abstract items to make things like this invisible.
        private uint shaderID;
        private GL _gl;
        uint fragmentID;
        uint vertexID;
        public Shader(GL gl, string vertexPath, string fragmentPath)
        {
            _gl = gl;

            //Load the individual shaders.
            vertexID = LoadShader(ShaderType.VertexShader, vertexPath);
            fragmentID = LoadShader(ShaderType.FragmentShader, fragmentPath);
            //Create the shader program.
            shaderID = _gl.CreateProgram();
            //Attach the individual shaders.
            _gl.AttachShader(shaderID, vertexID);
            _gl.AttachShader(shaderID, fragmentID);
            _gl.LinkProgram(shaderID);
            //Check for linking errors.
            _gl.GetProgram(shaderID, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Program failed to link with error: {_gl.GetProgramInfoLog(shaderID)}");
            }
            //Detach and delete the shaders
            _gl.DetachShader(shaderID, vertexID);
            _gl.DetachShader(shaderID, fragmentID);
            _gl.DeleteShader(vertexID);
            _gl.DeleteShader(fragmentID);
        }

        public void Use()
        {
            //Using the program
            _gl.UseProgram(shaderID);
        }

        public void detach()
        {
            //Using the program
            _gl.UseProgram(0);
        }

        //Uniforms are properties that applies to the entire geometry
        public void SetUniform(string name, int value)
        {
            //Setting a uniform on a shader using a name.
            int location = _gl.GetUniformLocation(shaderID, name);
            if (location == -1) //If GetUniformLocation returns -1 the uniform is not found.
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            // Use();
            _gl.Uniform1(location, value);
        }

        public void SetUniform(string name, float value)
        {
            int location = _gl.GetUniformLocation(shaderID, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, value);
        }
        public void SetUniform(string name, int[] array)
        {
            int location = _gl.GetUniformLocation(shaderID, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            _gl.Uniform1(location, array);
        }

        public unsafe void SetUniform(string name, Matrix4x4 value)
        {
            //A new overload has been created for setting a uniform so we can use the transform in our shader.
            int location = _gl.GetUniformLocation(shaderID, name);
            if (location == -1)
            {
                throw new Exception($"{name} uniform not found on shader.");
            }
            Use();
            _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }


        public void Dispose()
        {

            //Remember to delete the program when we are done.
            _gl.DeleteProgram(shaderID);
        }

        private uint LoadShader(ShaderType type, string path)
        {

            //To load a single shader we need to:
            //1) Load the shader from a file.
            //2) Create the handle.
            //3) Upload the source to opengl.
            //4) Compile the shader.
            //5) Check for errors.
            string src = File.ReadAllText(path);
            // Console.WriteLine(src);
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            string infoLog = _gl.GetShaderInfoLog(handle);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling shader of type {type}, failed with error {infoLog}");
            }

            return handle;
        }
    }
}
