using System.Diagnostics;

namespace GameEngine.Core
{
    public class TimeManager
    {
        private Stopwatch _stopwatch;
        private float _beginTime;

        public void Initialize()
        {
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
            _beginTime = 0;
        }

        public void Update()
        {
            Time.time = (float)_stopwatch.Elapsed.TotalSeconds;
            Time.deltaTime = Time.time - _beginTime;
            _beginTime = Time.time;
        }
    }
}
