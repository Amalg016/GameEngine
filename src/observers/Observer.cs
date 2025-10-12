using GameEngine.observers.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.observers
{
    public interface Observer
    {
       void onNotify(GameObject obj,Event _event);
    }
}
