using GameEngine.observers.events;
using System.Collections.Generic;

namespace GameEngine.observers
{
    public class EventSystem
    {
        static List<Observer> observers = new List<Observer>(); 
        public static void addObserver(Observer observer)
        {
            observers.Add(observer);    
        }
        public static void notify(GameObject obj,Event _event)
        {
            foreach (var observer in observers)
            {
                observer.onNotify(obj, _event);
            }
        }
    }
}
