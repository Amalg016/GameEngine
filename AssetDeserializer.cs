using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.components;
using GameEngine.Physics2D.components;
using GameEngine.util;

namespace GameEngine
{
    public class AssetDeserializer : JsonConverter
    {
        static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        { ContractResolver = new AssetSpecifiedClassConvertor() };
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





        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            //     Console.WriteLine(jo["Name"].Value<string>());
                string name = jo["Name"].Value<string>();
            JArray objects = jo["frames"].Value<JArray>();
            //   JObject transform = jo["transform"].Value<JObject>();
            //    Transform t = serializer.Deserialize<Transform>(transform.CreateReader());
            bool Zindex = jo["Loop"].Value<bool>();
            float speed = jo["speed"].Value<float>();
            Animation go = new Animation();
                go.Loop = Zindex;
                go.Name = name;
            go.frames = new List<Frame>();
            foreach (var item in objects)
            {
                Frame frame=new Frame();
                float frameRate = item["frameRate"].Value<float>();
                frame.frameRate=frameRate;  
                JObject sprite = item["sprite"].Value<JObject>();
                string Sname = sprite["SpritesheetName"].Value<string>();
                int index = sprite["Spritesheet_index"].Value<int>();
                frame.sprite = (AssetPool.TryFindSpriteSheet(Sname)).GetSprite(index);
                go.AddFrame(frame);

            }

            return go;


        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Animation));

        }

        public class AssetSpecifiedClassConvertor : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (typeof(Animation).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                    return null;
                return base.ResolveContractConverter(objectType);
            }
        }
    }
}

