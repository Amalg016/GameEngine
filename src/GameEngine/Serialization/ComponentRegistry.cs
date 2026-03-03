using GameEngine.components;

namespace GameEngine.Serialization
{
    /// <summary>
    /// Registry for custom component types. Games and editor register their component types here
    /// so the ComponentDeserializer can deserialize them from scene files.
    /// 
    /// Usage:
    ///   ComponentRegistry.Register&lt;PlayerController&gt;();
    ///   ComponentRegistry.Register&lt;EnemyAI&gt;();
    /// </summary>
    public static class ComponentRegistry
    {
        private static readonly Dictionary<string, Type> _registry = new();

        /// <summary>
        /// Register a component type by its full name (namespace.TypeName).
        /// </summary>
        public static void Register<T>() where T : Component
        {
            string key = typeof(T).FullName ?? typeof(T).Name;
            _registry[key] = typeof(T);
        }

        /// <summary>
        /// Register a component type with an explicit name key.
        /// </summary>
        public static void Register(string name, Type type)
        {
            _registry[name] = type;
        }

        /// <summary>
        /// Try to resolve a component type by its serialized name.
        /// </summary>
        public static Type Resolve(string name)
        {
            return _registry.TryGetValue(name, out var type) ? type : null;
        }

        /// <summary>
        /// Get all registered component types (for editor UI).
        /// </summary>
        public static IReadOnlyDictionary<string, Type> GetAllRegistered()
        {
            return _registry;
        }

        /// <summary>
        /// Auto-register all Component subclasses found in the given assemblies.
        /// </summary>
        public static void RegisterAllFromAssembly(System.Reflection.Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    string key = type.FullName ?? type.Name;
                    _registry[key] = type;
                }
            }
        }
    }
}
