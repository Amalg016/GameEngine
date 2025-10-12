using GameEngine.components;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using GameEngine.ECS;

namespace GameEngine.Serialization
{
    public class GameObjectDeserializer : JsonConverter<GameObject>
    {
        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        { ContractResolver = new GameObjectSpecifiedClassConvertor() };
        //   public override bool CanConvert(Type objectType)
        //   {
        //       return (objectType == typeof(GameObject));  
        //   }

        //      public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        //      {
        //          try
        //          {
        //              if (reader == null)
        //              {
        //                  throw new ArgumentNullException();
        //              }
        //              JObject jo = JObject.Load(reader);
        //              //   Console.WriteLine(jo["Name"].Value<string>());
        //              string name = jo["Name"].Value<string>();
        //              IList<JsonReader> objects = jo["components"].Value<IList<JsonReader>>();
        //              Transform transform = jo["transform"].Value<Transform>();
        //              int Zindex = jo["ZIndex"].Value<int>();
        //
        //              GameObject go = new GameObject();
        //              go.Iniit(name, transform, Zindex);
        //              foreach (var item in objects)
        //              {
        //                  Component c = serializer.Deserialize<Component>((item));
        //                  go.AddComponent(c);
        //              }
        //              return go;
        //          }
        //          catch(Exception e)
        //          {
        //              Console.WriteLine(e.Message);
        //          }
        //           return null;
        //      }
        public override bool CanWrite
        {
            get { return false; }
            // get { return true; }
        }



        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {

            JObject jo = JObject.Load(reader);
            //     Console.WriteLine(jo["Name"].Value<string>());
            string name = jo["name"].Value<string>();
            JArray objects = jo["components"].Value<JArray>();
            //   JObject transform = jo["transform"].Value<JObject>();
            //    Transform t = serializer.Deserialize<Transform>(transform.CreateReader());
            int Zindex = jo["ZIndex"].Value<int>();
            int uid = jo["uid"].Value<int>();
            GameObject go = new GameObject();
            go.Load(name, Zindex, uid);
            foreach (var item in objects)
            {
                Component c = serializer.Deserialize<Component>((item.CreateReader()));
                if (c is Transform)
                {
                    go.transform.position = ((Transform)c).position;
                    go.transform.rotation = ((Transform)c).rotation;
                    go.transform.scale = ((Transform)c).scale;
                }
                else
                {
                    go.LoadAddComponent(c);
                }
            }
            return go;

        }
    }

    public class GameObjectSpecifiedClassConvertor : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(GameObject).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }
}
