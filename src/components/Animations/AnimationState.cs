using GameEngine.ECS;

namespace GameEngine.components.Animations
{
    public class AnimationState
    {
        public string Name = "default";
        // public Transition[] transitions;
        public Animation animation;
        public int nextState;
        public Guid guid = new Guid();

        public enum ParameterType { Bool, Float, Int };
        public List<ParameterType> parameterTypes = new List<ParameterType>();
        public Dictionary<int, int> IntParameters = new Dictionary<int, int>();
        public Dictionary<int, float> FloatParameters = new Dictionary<int, float>();
        public Dictionary<int, bool> BoolParameters = new Dictionary<int, bool>();

        AnimationController controller;
        public void Init(AnimationController controller)
        {
            this.controller = controller;
        }
        public void Update(GameObject go)
        {
            if (animation == null) return;
            animation.Update(go);
        }

        public bool CheckForValidity()
        {
            for (int i = 0; i < IntParameters.Count; i++)
            {
                KeyValuePair<int, int> s = IntParameters.ElementAt(i);
                if (controller.Int.ElementAt(s.Key).Value != s.Value)
                {
                    return false;
                }
            }
            for (int i = 0; i < FloatParameters.Count; i++)
            {
                KeyValuePair<int, float> s = FloatParameters.ElementAt(i);
                if (controller.Float.ElementAt(s.Key).Value != s.Value)
                {
                    return false;
                }
            }
            for (int i = 0; i < BoolParameters.Count; i++)
            {
                KeyValuePair<int, bool> s = BoolParameters.ElementAt(i);
                if (controller.Bool.ElementAt(s.Key).Value != s.Value)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
