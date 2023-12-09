using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using GameEngine.components;
using ImGuiNET;
using GameEngine.editor;

namespace GameEngine
{
    [System.Serializable]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [JsonConverter(typeof(GameObjectDeserializer))]
    public class GameObject
    {
        private static int ID_Counter = 0;
        [JsonRequired] private int uid = -1;

        [JsonRequired]
        public string name;
        [JsonRequired]
        private List<Component> components = new List<Component>();


        [JsonIgnore][NonSerialized] public Transform transform;
        [NonSerialized]  private  bool serialize = true;
        //  [XmlInclude(ty)]
        //  [XmlElement]
        [JsonRequired]
        private int ZIndex { get; set; }
        [JsonIgnore] public bool isDead=false;
       [JsonRequired] public bool isPrefab=false;
        public GameObject()
        {
            transform = new Transform();
            AddComponent(transform);
        }
        
        public void Iniit(String name, int zind)
        {
            this.name = name;
           // this.transform = transform;
         
            this.ZIndex = zind;
            this.uid = ID_Counter++;   
        }
        public void LoadInit(String name, int zind,int id)
        {
            this.name = name;
            // this.transform = transform;
            isDead = false;
            this.ZIndex = zind;
            uid = id;
        }


        public T GetComponent<T>() where T : Component
        {
            try
            {
                foreach (var component in components)
                {
                    if (component is T)
                    {
                        return (T)component ;
                    }
                }
                return null ;   

            }
              catch(Exception e)
            {
                //Debug.log()
            }
                return null;

        }

        public void RemoveComponent<T>()
        {
            foreach (var item in components)
            {
                if ( item is T)
                {
                    components.Remove(item);
                    return;
                }
            }
        }

        public void AddComponent(Component c)
        {
            c.generateId();
            this.components.Add(c);
            c.gameObject = this;    
        }

        public void LoadAddComponent(Component c)
        {
            this.components.Add(c);
            c.gameObject = this;    
        }


        public void Update()
        {
            foreach (var item in components)
            {
                item.Update();
            }
        }

        public void Load()
        {
            foreach (var item in components)
            {
                item.Load();
            }
        }

        public int zIndex()
        {
            return this.ZIndex;
        }

        public void IMGUI()
        {
            name = AJGui.inputText("Name", name);
           // var val =ZIndex;
            ZIndex = AJGui.dragInt("Layer:", ZIndex);
          //  if(ImGui.DragInt("Layer:",ref val))
          //  {
          //      ZIndex = val;
          //  }
           
            foreach (var item in components)
            {
                ImGuiTreeNodeFlags treeNodeFlags = ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding|ImGuiTreeNodeFlags.AllowOverlap;
               if(ImGui.CollapsingHeader(item.GetType().Name,treeNodeFlags))
                item.gui();
            }
        }
      
        public static void setIdCounter(int maxId)
        {
            ID_Counter = maxId;
        }
        public int getUid()
        {
            return this.uid;
        }
        public List<Component> getAllComponents()// where T : Component
        {
            return components;
        }

        public void dontSerialize()
        {
            serialize = false;
        }
        public void Serialize()
        {
            serialize = true;
        }
        public bool doSerialize()
        {
           return serialize;
        }

        public void Destroy()
        {
            this.isDead = true;
            foreach (var item in components)
            {
                item.Destroy();
            }
        }

        public void EditorUpdate()
        {
            foreach (var item in components)
            {
                item.EditorUpdate();
            }
        }

        public GameObject Copy()
        {
            string  blueprint = JsonConvert.SerializeObject(this);
            GameObject clone= JsonConvert.DeserializeObject<GameObject>(blueprint, new GameObjectDeserializer(), new ComponentDeserializer(), new ColliderDeserializer());
            clone.uid = ID_Counter++;
            foreach (var item in clone.getAllComponents())
            {
                item.generateId();
            }
            Window.GetScene().addGameObjectToScene(clone);  
            return clone;   
        }
    }
}
