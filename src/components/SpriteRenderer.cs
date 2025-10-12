using GameEngine.Rendering.Core;
using Newtonsoft.Json;
using System.Numerics;

namespace GameEngine.components
{
    //  [JsonObject(MemberSerialization.OptIn)]
    [System.Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SpriteRenderer : Component
    {

        //  [JsonRequired]
        [JsonRequired] public Sprite sprite;
        // [JsonRequired]
        private Transform lastTransform = new Transform();
        [NonSerialized]
        public bool IsDirty = true;

        [JsonRequired] private Vector4 color;

        public SpriteRenderer()
        {
            IsDirty = true;
        }

        public void init(Sprite sprite)
        {
            this.sprite = sprite;
            this.color = new Vector4(1, 1, 1, 1);
            IsDirty = true;
        }
        public override void Load()
        {
            this.lastTransform = gameObject.transform.copy();

        }

        public override void Update()
        {
            if (!lastTransform.Equals(this.gameObject.transform))
            {
                this.gameObject.transform.copy(lastTransform);
                IsDirty = true;
            }
        }
        public override void EditorUpdate()
        {
            if (!lastTransform.Equals(this.gameObject.transform))
            {
                this.gameObject.transform.copy(lastTransform);
                IsDirty = true;
            }
        }
        public Vector4 GetColor()
        {
            return color;
        }
        public Texture GetTexture()
        {
            return sprite.getTexture();
        }
        public Vector2[] GetTexCoords()
        {
            return sprite.getTexCoords();
        }

        public void setSprite(Sprite sprite)
        {
            this.sprite = sprite;

            IsDirty = true;
        }
        public void setColor(Vector4 color)
        {
            if (!this.color.Equals(color))
            {
                IsDirty = true;
                this.color = color;
            }
        }

        public bool isDirty()
        {
            return IsDirty;
        }
        public void setDirty()
        {
            IsDirty = true;
        }

        public void setClean()
        {
            IsDirty = false;
        }

    }
}
