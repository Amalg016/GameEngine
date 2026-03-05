using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using GameEngine.components;
using GameEngine.components.Animations;
using GameEngine.Core.Utilities;
using System.Collections.Generic;
using System.Numerics;

namespace GameEngine.Serialization
{
    public class AnimationControllerDeserializer : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AnimationController);
        }

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            AnimationController controller = new AnimationController();
            controller.Name = jo["Name"]?.Value<string>() ?? "default";

            // Deserialize parameters
            if (jo["Bool"] != null)
            {
                controller.Bool = JsonConvert.DeserializeObject<Dictionary<string, bool>>(jo["Bool"].ToString()) 
                    ?? new Dictionary<string, bool>();
            }
            if (jo["Int"] != null)
            {
                controller.Int = JsonConvert.DeserializeObject<Dictionary<string, int>>(jo["Int"].ToString()) 
                    ?? new Dictionary<string, int>();
            }
            if (jo["Float"] != null)
            {
                controller.Float = JsonConvert.DeserializeObject<Dictionary<string, float>>(jo["Float"].ToString()) 
                    ?? new Dictionary<string, float>();
            }

            // Deserialize states
            if (jo["States"] != null)
            {
                JArray statesArray = jo["States"].Value<JArray>();
                foreach (var stateToken in statesArray)
                {
                    AnimationState state = new AnimationState();
                    state.Name = stateToken["Name"]?.Value<string>() ?? "default";

                    // Deserialize editor position
                    if (stateToken["EditorPosition"] != null)
                    {
                        float posX = stateToken["EditorPosition"]?["X"]?.Value<float>() ?? 0f;
                        float posY = stateToken["EditorPosition"]?["Y"]?.Value<float>() ?? 0f;
                        state.EditorPosition = new Vector2(posX, posY);
                    }

                    // Link animation by name
                    string animName = stateToken["animation"]?["Name"]?.Value<string>();
                    if (animName != null)
                    {
                        Animation anim = AssetPool.TryGetAnimation(animName);
                        state.animation = anim;
                    }

                    // Deserialize transitions
                    if (stateToken["Transitions"] != null)
                    {
                        JArray transArray = stateToken["Transitions"].Value<JArray>();
                        foreach (var transToken in transArray)
                        {
                            StateTransition transition = new StateTransition();
                            transition.NextStateIndex = transToken["NextStateIndex"]?.Value<int>() ?? 0;

                            if (transToken["IntParameters"] != null)
                            {
                                transition.IntParameters = JsonConvert.DeserializeObject<Dictionary<string, int>>(transToken["IntParameters"].ToString())
                                    ?? new Dictionary<string, int>();
                            }
                            if (transToken["FloatParameters"] != null)
                            {
                                transition.FloatParameters = JsonConvert.DeserializeObject<Dictionary<string, float>>(transToken["FloatParameters"].ToString())
                                    ?? new Dictionary<string, float>();
                            }
                            if (transToken["BoolParameters"] != null)
                            {
                                transition.BoolParameters = JsonConvert.DeserializeObject<Dictionary<string, bool>>(transToken["BoolParameters"].ToString())
                                    ?? new Dictionary<string, bool>();
                            }

                            state.Transitions.Add(transition);
                        }
                    }

                    controller.States.Add(state);
                }
            }

            return controller;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
