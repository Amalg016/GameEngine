using GameEngine.components;
using GameEngine.Physics2D.components;
using GameEngine.util;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine
{
    public class ColliderDeserializer:JsonConverter<Collider>
    {
        
            static JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            { ContractResolver = new ColliderSpecifiedClassConvertor() };
            
            public override bool CanWrite
            {
                get { return false; }
                // get { return true; }
            }


        public override void WriteJson(JsonWriter writer, Collider value, JsonSerializer serializer)
        {
            //writer.WriteValue(value.ToString());


            writer.Formatting = Formatting.Indented;
         
            writer.WriteStartObject();
            writer.WritePropertyName("Name");
            writer.WriteValue(value.GetType().Name);
            writer.WritePropertyName("Properties");
            writer.WriteValue((object)value);
            writer.WriteEndObject();

            //     //  writer.
               // JToken t = JToken.((object)value);
               //
               // if (t.Type != JTokenType.Object)
               // {
               //     t.WriteTo(writer);
               // }
         //       else
         //       {
         //           JObject o = (JObject)t;
         //           IList<string> propertyNames = o.Properties().Select(p => p.Name).ToList();
         //   
         //           o.AddFirst(new JProperty(value.GetType().Name, new JArray(propertyNames)));
         //   
         //           o.WriteTo(writer);
         //       
         //       }
        }

        public override Collider ReadJson(JsonReader reader, Type objectType, Collider existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            
           
            JObject jo = JObject.Load(reader);
            //  Console.WriteLine(jo["Name"].Value<string>());
            switch (jo["Name"].Value<string>())
            {
                case "GameEngine.Physics2D.components.CircleCollider":
                    return JsonConvert.DeserializeObject<CircleCollider>(jo.ToString(), serializerSettings);
                case "GameEngine.Physics2D.components.Box2DCollider":
                    return JsonConvert.DeserializeObject<Box2DCollider>(jo.ToString(), serializerSettings);

                default:
                    throw new Exception("new component serialization not added");
            }
            throw new Exception("Exception from component deserialization");

        }
    }

        public class ColliderSpecifiedClassConvertor : DefaultContractResolver
        {
            protected override JsonConverter ResolveContractConverter(Type objectType)
            {
                if (typeof(Collider).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                    return null;
                return base.ResolveContractConverter(objectType);
            }
        }

}


