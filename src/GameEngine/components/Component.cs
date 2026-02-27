using GameEngine.ECS;
using GameEngine.Serialization;
using Newtonsoft.Json;
using System.Numerics;
using System.Reflection;

namespace GameEngine.components
{

    // [JsonObject(MemberSerialization.OptIn)]
    //[JsonSubtypes.KnownSubType(typeof(FontRenderer), true)]
    [System.Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [JsonConverter(typeof(ComponentDeserializer))]
    public abstract class Component
    {

        //   [NonSerialized]
        public static int ID_Counter = 0;
        [JsonRequired] private int uid = -1;
        [JsonIgnore]
        public GameObject gameObject = null;
        [JsonRequired]
        string Name { get { return this.ToString(); } }
        public virtual void Load() { }

        public virtual void Update() { }

        public virtual void gui()
        {
            // Override in editor or use EditorInspector for ImGui rendering
        }

        private int indexof(string enumType, string[] enumValues)
        {
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumType == enumValues[i])
                {
                    return i;
                }
            }
            return 0;
        }

        private string[] getEnumValues(Type type)
        {
            string[] enumValues = new string[type.GetEnumValues().Length];
            int i = 0;
            foreach (var t in type.GetEnumNames())
            {
                enumValues[i] = t;
                i++;
            }
            return enumValues;
        }

        public void generateId()
        {
            if (this.uid == -1)
            {
                this.uid = ID_Counter++;
            }
        }
        public int getUid()
        {
            return this.uid;
        }
        public static void init(int maxID)
        {
            ID_Counter = maxID;
        }

        public void Destroy()
        {

        }

        public virtual void EditorUpdate() { }
    }
}
