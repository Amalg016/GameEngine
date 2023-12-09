using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.components
{
    public class GizmoSystem : Component
    {
        private Spritesheet gizmos;
        private int usingGizmo = 0;

        public GizmoSystem(Spritesheet gizmos)
        {
            this.gizmos = gizmos;
         //   gameObject.AddComponent(new Rigidbody());
            }

        public override void Load()
        {
            if (gameObject == null) return;
        }

        public override void EditorUpdate()
        {
            if(usingGizmo==0)
            {
                gameObject.GetComponent<TranslateGizmo>().setUsing();
                gameObject.GetComponent<ScaleGizmo>().setNotUsing();
            }
            else if (usingGizmo == 1)
            {
                gameObject.GetComponent<TranslateGizmo>().setNotUsing();
                gameObject.GetComponent<ScaleGizmo>().setUsing();
            }

            if (InputManager.E)
            {
                usingGizmo = 0;
            }
            else if (InputManager.R)
            {
                usingGizmo = 1;
            }
        }
    }
}
