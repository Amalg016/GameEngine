using System.Collections.Generic;

namespace GameEngine.components.Animations
{
    public class StateTransition
    {
        public int NextStateIndex;
        // Conditions required to take this transition
        public Dictionary<string, int> IntParameters = new Dictionary<string, int>();
        public Dictionary<string, float> FloatParameters = new Dictionary<string, float>();
        public Dictionary<string, bool> BoolParameters = new Dictionary<string, bool>();

        public bool CheckCondition(AnimationController controller)
        {
            if (controller == null) return false;

            foreach (var s in IntParameters)
            {
                if (!controller.Int.TryGetValue(s.Key, out int val) || val != s.Value) return false;
            }
            foreach (var s in FloatParameters)
            {
                if (!controller.Float.TryGetValue(s.Key, out float val) || val != s.Value) return false;
            }
            foreach (var s in BoolParameters)
            {
                if (!controller.Bool.TryGetValue(s.Key, out bool val) || val != s.Value) return false;
            }
            return true;
        }
    }
}
