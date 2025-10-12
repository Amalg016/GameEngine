using GameEngine.components;
using GameEngine.Physics2D.components;
using GameEngine.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Numerics;

namespace GameEngine.Serialization
{
    public class ComponentDeserializer : JsonConverter
    {
        //   public static 
        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        { ContractResolver = new ComponentSpecifiedClassConvertor() };
        //  static JsonSerializerSettings CollserializerSettings = new JsonSerializerSettings() 
        //  {ContractResolver=new ColliderSpecifiedClassConvertor() };    
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Component)) && (objectType != typeof(Collider));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            // switch (objectType.Name) 
            switch (jo["Name"].Value<string>())
            {
                case "GameEngine.components.SpriteRenderer":
                    SpriteRenderer renderer = JsonConvert.DeserializeObject<SpriteRenderer>(jo.ToString(), serializerSettings);
                    JObject col = jo["color"].Value<JObject>();
                    //JsonSchema json = 
                    Vector4 color = JsonConvert.DeserializeObject<Vector4>(col.ToString());
                    JObject sprite = jo["sprite"].Value<JObject>();
                    string name = sprite["SpritesheetName"].Value<string>();
                    int index = sprite["Spritesheet_index"].Value<int>();
                    //       string path1 = sprite["path"].Value<string>();
                    if (name != "")
                    {
                        renderer.init((AssetPool.TryFindSpriteSheet(name)).GetSprite(index));
                        renderer.setColor(color);
                    }
                    else
                    {
                        //     Sprite sprite1 = new Sprite();
                        //     sprite1.SetTexture(AssetPool.getTexture(path1, path1));
                        //       renderer.init(sprite1);
                    }
                    return renderer;
                case "GameEngine.components.FontRenderer":
                    return JsonConvert.DeserializeObject<FontRenderer>(jo.ToString(), serializerSettings);
                case "GameEngine.components.EditorCamera":
                    return JsonConvert.DeserializeObject<EditorCamera>(jo.ToString(), serializerSettings);
                case "GameEngine.Physics2D.components.Rigidbody2D":
                    return JsonConvert.DeserializeObject<Rigidbody2D>(jo.ToString(), serializerSettings);
                case "GameEngine.components.Animator":
                    //    Animator animator= JsonConvert.DeserializeObject<Animator>(jo.ToString(),serializerSettings);
                    List<string> names = new List<string>();
                    //           foreach (var item in animator.getAllAnimations())
                    //           {
                    //               names.Add(item.Name);
                    //           }
                    //            animator.clearAllAnimations();
                    //           foreach (var item in names)
                    //           {
                    //              animator.AddClip(AssetPool.TryGetAnimation(item));
                    //           }
                    JObject contro = jo["controller"].Value<JObject>();
                    string Name = contro["Name"].Value<string>();

                    ;
                    Animator animator = new Animator();
                    if (AssetPool.TryFindController(Name) != null)
                    {
                        animator.controller = AssetPool.TryFindController(Name);
                    }
                    return animator;
                case "GameEngine.Physics2D.components.CircleCollider":
                    return JsonConvert.DeserializeObject<Collider>(jo.ToString());
                case "GameEngine.Physics2D.components.Box2DCollider":
                    return JsonConvert.DeserializeObject<Collider>(jo.ToString());

                case "GameEngine.components.Transform":
                    JObject pos = jo["position"].Value<JObject>();
                    JObject size = jo["scale"].Value<JObject>();
                    Vector2 position = JsonConvert.DeserializeObject<Vector2>(pos.ToString());
                    Vector2 scale = JsonConvert.DeserializeObject<Vector2>(size.ToString());

                    float rot = jo["rotation"].Value<float>();
                    Transform transform = new Transform(position, scale);
                    transform.rotation = rot;
                    return transform;
                default:
                    throw new Exception("new component serialization not added");
            }
            throw new Exception("Exception from component deserialization");
        }
        public override bool CanWrite
        {
            get { return false; }
            // get { return true; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {



        }
    }

    public class ComponentSpecifiedClassConvertor : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Component).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }
}
