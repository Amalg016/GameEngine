using GameEngine.components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics2D.components
{
    [System.Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [JsonConverter(typeof(ColliderDeserializer))]
    public abstract class Collider : Component
    {
       [JsonRequired] public Vector2 Offset = new Vector2();       
    }
}
