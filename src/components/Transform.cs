using Newtonsoft.Json;
using System.Numerics;


namespace GameEngine.components
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class Transform : Component
    {
        [JsonRequired] public Vector2 position;
        [JsonRequired] public Vector2 scale;
        [JsonRequired] public float rotation=0.0f;

        /// [JsonConstructor]
        public Transform()
        {
            init(new Vector2(), new Vector2());
        }
        [JsonConstructor]    
        public Transform(Vector2 position, Vector2 scale)
        {
            init(position, scale);
        }
        
        public void init(Vector2 pos,Vector2 scal)
        {
            position=pos;
            scale = scal;
        }

        public Transform copy()
        {
            Transform t= new Transform(new Vector2(this.position.X, this.position.Y), new Vector2(this.scale.X, this.scale.Y));
         t.rotation=rotation;   
            return t;
        }

        public void copy(Transform to)
        {
            to.position.X = this.position.X;
            to.position.Y = this.position.Y;

            to.scale.X = this.scale.X;
            to.scale.Y = this.scale.Y;
            to.rotation=rotation;   
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(Transform)) return false;
            Transform t = obj as Transform;
            return t.position.Equals(this.position) &&t.scale.Equals(this.scale) && t.rotation==this.rotation;
        }

        public override void gui()
        {
            base.gui();
        }
    }
}
