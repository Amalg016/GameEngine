using GameEngine.components;
using System.Numerics;

namespace GameEngine
{
    public class Prefab
    {
        static int no = 0;
        public static GameObject generateSpriteObject(Sprite sprite, float sizeX, float sizeY, int zindex)
        {

            GameObject block = new GameObject();
            Console.WriteLine("prefab");
            block.Load($"Sprite_Objeet_Gen {no}", zindex);
            SpriteRenderer renderer = new SpriteRenderer();
            block.transform.position = new Vector2(200, 200);
            block.transform.scale = new Vector2(sizeX, sizeY);
            // block.transform.rotation = 0;
            renderer.init(sprite);
            block.AddComponent(renderer);
            no++;
            return block;
        }



    }
}
