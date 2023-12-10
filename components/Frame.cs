using Newtonsoft.Json;

namespace GameEngine.components
{
    public class Frame
    {
      [JsonRequired]  public Sprite sprite;
       [JsonRequired] public float frameRate;
    }
}
