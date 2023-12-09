using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.scenes
{
    public abstract class sceneInitializer
    {
        public abstract void Init(Scene scene);
        public abstract void loadResources(Scene scene);
        public abstract void imgui();
        public abstract void Exit(Scene scene);

    }
}
