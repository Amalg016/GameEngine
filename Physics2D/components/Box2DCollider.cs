using GameEngine.components;
using GameEngine.renderer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics2D.components
{
    public class Box2DCollider : Collider
    {
      [JsonRequired]   public  Vector2 halfSize = new Vector2(1,1);
     [JsonRequired]   public Vector2 Orgin = new Vector2();

        public override void Load()
        {
        }

        public override void EditorUpdate()
        {
            Vector2 center = new Vector2(gameObject.transform.position.X + Offset.X, gameObject.transform.position.Y + Offset.Y);
            if (!gameObject.isDead)
            {
              DebugDraw.addBox2D(center, this.halfSize, gameObject.transform.rotation);
            }
        }
    } 
}
