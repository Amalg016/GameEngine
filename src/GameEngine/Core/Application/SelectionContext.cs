using GameEngine.ECS;

namespace GameEngine.Core.Application
{
    /// <summary>
    /// Engine-level selection context. Tracks the currently selected/active game object.
    /// The editor wires this up to PropertiesWindow; game code can ignore or use it.
    /// </summary>
    public static class SelectionContext
    {
        private static GameObject _activeGameObject;

        public static event Action<GameObject> OnSelectionChanged;

        public static GameObject ActiveGameObject
        {
            get => _activeGameObject;
            set
            {
                if (_activeGameObject != value)
                {
                    _activeGameObject = value;
                    OnSelectionChanged?.Invoke(_activeGameObject);
                }
            }
        }
    }
}
