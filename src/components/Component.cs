using GameEngine.ECS;
using GameEngine.Editor;
using GameEngine.Editor.Window;
using GameEngine.Serialization;
using ImGuiNET;
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
            try
            {
                //  ImGui.BeginMenu(this.ToString());
                FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var field in fields)
                {
                    if (field.IsNotSerialized)
                    {
                        continue;
                    }
                    Type type = field.FieldType;

                    //  Console.WriteLine(type);    
                    Object value = field.GetValue(this);
                    string name = field.Name;
                    if (type == typeof(int))
                    {
                        int val = (int)value;
                        field.SetValue(this, AJGui.dragInt(name, val));

                    }
                    else if (type == typeof(float))
                    {
                        float val = (float)value;

                        field.SetValue(this, AJGui.dragFloat(name, val));

                    }
                    else if (type == typeof(bool))
                    {
                        bool val = (bool)value;
                        if (ImGui.Checkbox(name + ":", ref val))
                        {
                            field.SetValue(this, !val);
                        }
                    }
                    else if (type == typeof(Vector3))
                    {
                        Vector3 val = (Vector3)value;
                        float[] vec = { val.X, val.Y, val.Z };
                        if (ImGui.DragFloat3(name + ":", ref val))
                        {
                            field.SetValue(this, val);
                        }
                    }
                    else if (type == typeof(Vector2))
                    {
                        Vector2 val = (Vector2)value;
                        AJGui.drawVec2Control(name + ":", ref val);//ref field think about it
                        field.SetValue(this, val);
                    }
                    else if (name == "color")
                    {
                        Vector4 imColor = (Vector4)value;

                        if (ImGui.ColorEdit4("Color Picker", ref imColor))
                        {
                            field.SetValue(this, imColor);
                            this.gameObject.GetComponent<SpriteRenderer>().setDirty();
                        }
                        //    if (ImGui.ColorPicker4("Color Picker: ", ref imColor))
                        //    {
                        //        field.SetValue(this, imColor);
                        //        this.gameObject.GetComponent<SpriteRenderer>().setDirty();
                        //    }
                    }
                    else if (type == typeof(Vector4) && name != "color")
                    {
                        Vector4 val = (Vector4)value;
                        //  float[] vec = { val.X, val.Y, val.Z ,val.W};
                        if (ImGui.DragFloat4(name + ":", ref val))
                        {
                            field.SetValue(this, val);
                        }
                    }
                    else if (type.IsEnum)
                    {
                        string[] enumValues = getEnumValues(type);
                        string enumType = ((Enum)value).ToString();
                        int index = (indexof(enumType, enumValues));
                        Array s = type.GetEnumValues();
                        if (ImGui.Combo(field.Name, ref index, enumValues, enumValues.Length))
                        {
                            field.SetValue(this, s.GetValue(index));
                        }
                    }


                }



            }
            catch (Exception e)
            {
                throw new Exception();
                throw e;
            }
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
