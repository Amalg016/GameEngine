using GameEngine.renderer;
using Newtonsoft.Json;
using System.Numerics;

namespace GameEngine.components
{
    public class Sprite
    {
        [JsonRequired] public int Spritesheet_index;
      [JsonIgnore]  Spritesheet spritesheet;
        [JsonRequired] public string SpritesheetName;
      //  [JsonRequired] public string name;
        [JsonRequired] private string path;
        float height, width;
        [JsonRequired]    int textureID;
        private Texture texture;
        [JsonRequired]
        Vector2[] texCoords= {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 0)
        };

       

        public Texture getTexture()
        {
            return texture;
        }
        public Vector2[] getTexCoords()
        {
            return texCoords;
        } 
        public void SetSpritesheetIndex(int s)
        {
            Spritesheet_index = s;
        }
        public void SetSpritesheet(Spritesheet s)
        {
            spritesheet = s;
            SpritesheetName = s.Name;
        }
        public float GetHeight()
        {
            return height;
        }
        
        public float GetWidth()
        {
            return width;
        }
        
        public int GetTexID()
        {
            return texture==null? -1:(int)texture.getTexID();
        }

        public void SetHeight(float h)
        {
             height=h;
        }
        public void SetWidth(float w)
        {
             width=w;
        }
        
        public void SetTexture(Texture texture)
        {
            this.texture=texture;  
            path=texture.getFilePath();
        }
        public void SetTexCoords(Vector2[] s)
        {
            this.texCoords=s;
        }
    }
}
