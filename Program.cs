using GameEngine.Core.Application;

namespace GameEngine
{
    class Program
    {
        public static void Main(params string[] args)
        {
            GameEngineCore engineCore = GameEngineCore.Instance;
            engineCore.Start();
        }
    }
}
