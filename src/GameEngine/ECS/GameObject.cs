using Newtonsoft.Json;
using GameEngine.components;
using GameEngine.scenes;
using GameEngine.Serialization;

namespace GameEngine.ECS
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
        [NonSerialized] private bool serialize = true;
        //  [XmlInclude(ty)]
        //  [XmlElement]
        [JsonRequired]
        private int ZIndex { get; set; }
        [JsonIgnore] public bool isDead = false;
        [JsonRequired] public bool isPrefab = false;
        public GameObject()
        {
            transform = new Transform();
            AddComponent(transform);
        }

        public void Load(String name, int zind)
        {
            this.name = name;
            // this.transform = transform;

            this.ZIndex = zind;
            this.uid = ID_Counter++;
        }
        public void Load(String name, int zind, int id)
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
                        return (T)component;
                    }
                }
                return null;

            }
            catch (Exception e)
            {
                //Debug.log()
            }
            return null;

        }

        public Component GetComponent(Type type)
        {
            foreach (var component in components)
            {
                if (type.IsInstanceOfType(component))
                {
                    return component;
                }
            }
            return null;
        }

        public void RemoveComponent<T>()
        {
            foreach (var item in components)
            {
                if (item is T)
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
            // Rendered by EditorInspector in the editor project
        }

        public void setZIndex(int z)
        {
            ZIndex = z;
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
            string blueprint = JsonConvert.SerializeObject(this);
            GameObject clone = JsonConvert.DeserializeObject<GameObject>(blueprint, new GameObjectDeserializer(), new ComponentDeserializer(), new ColliderDeserializer());
            clone.uid = ID_Counter++;
            foreach (var item in clone.getAllComponents())
            {
                item.generateId();
            }
            SceneManager.CurrentScene.addGameObjectToScene(clone);
            return clone;
        }
    }
}
