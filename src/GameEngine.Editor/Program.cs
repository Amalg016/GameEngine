namespace GameEngine
{
    class Program
    {
        public static void Main(params string[] args)
        {
            EditorGameEngineCore engineCore = EditorGameEngineCore.Instance;
            engineCore.Start();
        }
    }
}
