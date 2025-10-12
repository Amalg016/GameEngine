using GameEngine.ECS;
using GameEngine.observers.events;

namespace GameEngine.observers
{
    public interface Observer
    {
        void onNotify(GameObject obj, Event _event);
    }
}
