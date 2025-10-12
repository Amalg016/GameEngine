using GameEngine.components;
using Newtonsoft.Json;
using System.Numerics;
using nkast.Aether.Physics2D.Dynamics;
using GameEngine.Core.Math;

namespace GameEngine.Physics2D.components
{
    public class Rigidbody2D : Component
    {
        [JsonRequired] public Vector2 velocity = new Vector2();
        [JsonRequired] public float angularDamping = 0;
        [JsonRequired] public float linearDamping = 0.9f;
        [JsonRequired] public float mass = 0;
        [JsonRequired] public BodyType bodyType = BodyType.Dynamic;
        [JsonRequired] public bool fixedRotation = false;
        [JsonRequired] public bool continuousCollision = true;
        public Body rawBody = null;
        [JsonRequired] string Name { get { return this.ToString(); } }


        public override void Load()
        {
            return;
        }
        public override void Update()
        {
            if (rawBody != null)
            {
                this.gameObject.transform.position.X = rawBody.Position.X;
                this.gameObject.transform.position.Y = rawBody.Position.Y;
                this.gameObject.transform.rotation = Mathf.ToDegrees(rawBody.Rotation);
            }

        }
    }
}
