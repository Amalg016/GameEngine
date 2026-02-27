using GameEngine.components.Animations;
using GameEngine.observers;
using GameEngine.observers.events;
using GameEngine.Core.Utilities;
using Newtonsoft.Json;
using GameEngine.ECS;

namespace GameEngine.components
{
    public class Animator : Component, Observer
    {
        [JsonRequired] public string Name { get { return this.ToString(); } }
        [JsonRequired] public AnimationController controller;
        string label;
        bool open = false;
        Animation anim = null;
        public void RefreshTextures()
        {
            if (controller == null) return;
            for (int i = 0; i < controller.States.Count; i++)
            {
                if (controller.States[i].animation != null)
                {
                    controller.States[i].animation.refreshTextures();
                }
            }
        }

        public override void gui()
        {
            // Rendered by EditorInspector in the editor project
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
        public string[] GetNames(List<Animation> animations)
        {
            string[] names = new string[animations.Count];
            for (int i = 0; i < animations.Count; i++)
            {
                names[i] = animations[i].Name;
            }
            return names;
        }
        public string[] GetNames(List<AnimationController> animations)
        {
            string[] names = new string[animations.Count];
            for (int i = 0; i < animations.Count; i++)
            {
                names[i] = animations[i].Name;
            }
            return names;
        }
        public override void Load()
        {
            RefreshTextures();
        }

        public override void Update()
        {

            //   base.Update();

            if (controller == null) return;
            controller.Update(gameObject);

            //animations[0].Update(gameObject);
        }
        public override void EditorUpdate()
        {
            //  Console.WriteLine(AssetPool.allAnimations.Count);
        }

        public void SetBool(string name, bool value)
        {
            if (controller.Bool.ContainsKey(name))
            {
                controller.Bool[name] = value;
            }
        }
        public void SetInt(string name, int value)
        {
            if (controller.Int.ContainsKey(name))
            {
                controller.Int[name] = value;
            }
        }
        public void Setfloat(string name, float value)
        {
            if (controller.Float.ContainsKey(name))
            {
                controller.Float[name] = value;
            }
        }
        public void onNotify(GameObject obj, Event _event)
        {
            if (_event.Type == EventType.UserEvent)
            {

            }
        }

    }
}
