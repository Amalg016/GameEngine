using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.components
{
    public class Frame
    {
      [JsonRequired]  public Sprite sprite;
       [JsonRequired] public float frameRate;
    }
}
