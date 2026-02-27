using GameEngine.components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics2D.components
{
    public class CircleCollider : Collider
    {
       [JsonRequired] public float radius = 1f;

        public override void Load()
        {

        }

        public override void Update()
        {
             
        }
    }
}
