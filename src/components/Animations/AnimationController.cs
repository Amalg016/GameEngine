using GameEngine.ECS;
using Newtonsoft.Json;
using System.Collections.Generic;

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
            
            int nextStateIndex = currentState.GetValidTransition();
            if (nextStateIndex >= 0 && nextStateIndex < States.Count)
            {
                currentState = States[nextStateIndex];
                if (currentState.animation != null)
                {
                    currentState.animation.Play();
                }
            }
        }

        private void Load()
        {
            foreach (var state in States)
            {
                state.Init(this);
            }
            if (States.Count > 0)
            {
                currentState = States[0];
                if (currentState.animation != null)
                {
                    currentState.animation.Play();
                }
            }
        }

        public void TransitionToState(AnimationState nextState)
        {
            if (nextState != null)
            {
                currentState = nextState;
                if (currentState.animation != null)
                {
                    currentState.animation.Play();
                }
            }
        }
    }
}
