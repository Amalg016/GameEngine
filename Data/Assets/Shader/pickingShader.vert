//Here we specify the version of our shader.
#version 330 core
//These lines specify the location and type of our attributes,
//the attributes here are prefixed with a "v" as they are our inputs to the vertex shader
//this isn't strictly necessary though, but a good habit.
layout (location = 0) in vec3 vPos;
layout (location = 1) in vec4 vColor;
layout (location = 2) in vec2 vTexCoords;
layout (location = 3) in float vTexId;
layout (location = 4) in float vEntityId;


//This is how we declare a uniform, they can be used in all our shaders and share the same name.
//This is prefixed with a u as it's our uniform.
//uniform float uBlue;
uniform mat4 uProjection;
uniform mat4 uView;

//This is our output variable, notice that this is prefixed with an f as it's the input of our fragment shader.
out vec4 fColor;
out vec2 fTexCoords;
out float fTexId;
out float fEntityId;
void main()
{
    //gl_Position, is a built-in variable on all vertex shaders that will specify the position of our vertex.
    fColor = vColor;
    fTexCoords=vTexCoords;
    fTexId=vTexId;
    fEntityId=vEntityId;
    gl_Position = uProjection * uView * vec4(vPos, 1.0);
  //  gl_Position = vec4(vPos, 1.0);
    //The rest of this code looks like plain old c (almost c#)
}
