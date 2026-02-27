using GameEngine.components;
using GameEngine.Serialization;
using Newtonsoft.Json;
using System.Numerics;

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
