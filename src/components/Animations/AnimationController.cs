using GameEngine.ECS;
using Newtonsoft.Json;

namespace GameEngine.components.Animations
{
    public class AnimationController
    {
        public string Name = "default";
        public Dictionary<string, bool> Bool = new Dictionary<string, bool>();
        public Dictionary<string, int> Int = new Dictionary<string, int>();
        public Dictionary<string, float> Float = new Dictionary<string, float>();
        public AnimationState currentState;
        [JsonRequired] public List<AnimationState> States = new List<AnimationState>();
        bool started = false;
        public void Update(GameObject go)
        {
            if (!started)
            {
                Load();
                started = true;
            }
            if (currentState == null) return;
            currentState.Update(go);
            if (!currentState.CheckForValidity())
            {
                currentState = States[currentState.nextState];
            }
        }

        private void Load()
        {
            if (States.Count > 0)
            {
                currentState = States[0];
            }
        }

        public void TransitionToState(AnimationState nextState)
        {
            if (nextState != null)
            {
                currentState = nextState;
            }
        }

    }
}
