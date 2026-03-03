using GameEngine;
using GameEngine.components;
using GameEngine.Core.Application;
using GameEngine.Core.Platform;
using Silk.NET.Input;

namespace SampleGame
{
    /// <summary>
    /// Basic top-down player controller. WASD to move in four directions.
    /// </summary>
    public class PlayerController : Component
    {
        public float speed = 3f;

        public override void Update()
        {
            if (InputManager.isKeyPressed(Key.W))
                gameObject.transform.position.Y += speed * Time.deltaTime;

            if (InputManager.isKeyPressed(Key.S))
                gameObject.transform.position.Y -= speed * Time.deltaTime;

            if (InputManager.isKeyPressed(Key.A))
                gameObject.transform.position.X -= speed * Time.deltaTime;

            if (InputManager.isKeyPressed(Key.D))
                gameObject.transform.position.X += speed * Time.deltaTime;
        }
    }
}
