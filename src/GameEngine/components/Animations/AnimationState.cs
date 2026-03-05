using GameEngine.ECS;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GameEngine.components.Animations
{
    public class AnimationState
    {
        public string Name = "default";
        public Animation animation;
        public Guid guid = new Guid();
        
        /// <summary>
        /// Position of this state node in the editor graph canvas.
        /// </summary>
        public Vector2 EditorPosition = Vector2.Zero;
        
        public List<StateTransition> Transitions = new List<StateTransition>();

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

        public int GetValidTransition()
        {
            if (controller == null) return -1;
            
            for (int i = 0; i < Transitions.Count; i++)
            {
                if (Transitions[i].CheckCondition(controller))
                {
                    return Transitions[i].NextStateIndex;
                }
            }
            return -1;
        }
    }
}
