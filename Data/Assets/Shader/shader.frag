#version 400
//The input variables, again prefixed with an f as they are the input variables of our fragment shader.
//These have to share name for now even though there is a way around this later on.
in vec4 fColor;
in vec2 fTexCoords; 
in float fTexId; 
uniform sampler2D uTextures[8];
  
//The output of our fragment shader, this just has to be a vec3 or a vec4, containing the color information about
//each "fragment" or pixel of our geometry.
out vec4 FragColor;

void main()
{
   if(fTexId > 0)
  {
    int id=int(fTexId);
    //Here we are setting our output variable, for which the name is not important.
    FragColor = fColor * texture(uTextures[id],fTexCoords);   
  }
  else
  {  
    FragColor = fColor;
  }
}
