using GameEngine.Rendering;
using System.Numerics;

namespace GameEngine.components
{
    public class Spritesheet
    {
        private Texture texture;
        List<Sprite> sprites = new List<Sprite>();
        public string Name;
        public Spritesheet(Texture texture, string name, float spriteWidth, float spriteHeight, int numSprites, int spacing)
        {
            Name = name;
            this.texture = texture;
            float currentX = 0;
            // int currentY = texture.getHeight()-spriteHeight;
            float currentY = 0;
            for (int i = 0; i < numSprites; i++)
            {
                float topY = (currentY + spriteHeight) / (float)texture.getHeight();
                float bottomY = currentY / (float)texture.getHeight();
                float rightX = (currentX + spriteWidth) / (float)texture.getWidth();
                float leftX = currentX / (float)texture.getWidth();

                Vector2[] texCoords = {
                new Vector2(rightX, bottomY),
                new Vector2(rightX, topY),
                new Vector2(leftX, topY),
                new Vector2(leftX, bottomY),
        };



                Sprite sprite = new Sprite();
                sprite.SetTexture(this.texture);
                sprite.SetTexCoords(texCoords);
                sprite.SetWidth(spriteWidth);
                sprite.SetHeight(spriteHeight);
                sprite.SetSpritesheetIndex(i);
                sprite.SetSpritesheet(this);
                this.sprites.Add(sprite);


                currentX += (spriteWidth + spacing);
                if (currentX >= texture.getWidth())
                {
                    currentX = 0;
                    currentY += spriteHeight + spacing;
                }
            }

        }
        public Sprite GetSprite(int index)
        {
            return this.sprites[index];
        }
        public int size()
        {
            return sprites.Count;
        }
    }
}
