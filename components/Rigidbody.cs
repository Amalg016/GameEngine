using GameEngine.Physics2D.enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.components
{
    public class Rigidbody : Component
    {
        [JsonRequired]
        private int colliderType = 0;
        [JsonRequired]
       // [NonSerialized]
        private float friction = 0.8f;
        [JsonRequired]
        public Vector3 velocity = new Vector3(0, 0.5f, 0);
      
        BodyType bodyType;

        public override void Load()
        {
        
        }
        public override void gui()
        {
            base.gui();
        }

        public override void Update()
        {
        
        }
    }
}
