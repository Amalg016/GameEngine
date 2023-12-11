using GameEngine.util;
using Newtonsoft.Json;

namespace GameEngine.components
{
    public class Animation
    {
        public string Name = "default";
       //  public Dictionary<Sprite,float> frames=new Dictionary<Sprite, float>();
        [JsonRequired] public List<Frame> frames = new List<Frame>();
        [JsonRequired] float speed = 1;
        public bool Loop = true;
        int currentIndex = 0;
        float timeTracker=0;
        [JsonIgnore] public Sprite currentSprite;
        
        public void AddFrame(Sprite sprite,float frameRate)
        {
            var frame = new Frame
            {
                sprite = sprite,
                frameRate = frameRate
            };
            frames.Add(frame);
        }
        public void AddFrame(Frame frame)
        {
            frames.Add(frame);
        }
        public void DeleteFrame(int frameIndex)
        {            
            frames.RemoveAt(frameIndex);
        }
        public void AddFrame(Spritesheet sheet, int index, float frameRate)
        {
            var frame = new Frame
            {
                sprite = sheet.GetSprite(index),
                frameRate = frameRate
            };
            frames.Add(frame);
        }
        
        public void Update(GameObject obj)
        {            
                timeTracker -= Time.deltaTime*speed;
                if(timeTracker <= 0)
                {
                    if (currentIndex < frames.Count-1)
                    {
                        currentIndex++;
                    }
                    else if(currentIndex==frames.Count-1||Loop)
                    {
                        currentIndex=(currentIndex+1)%frames.Count;
                    }

                    var s= frames[currentIndex];
                    timeTracker = s.frameRate;
                    currentSprite = s.sprite;
                    obj.GetComponent<SpriteRenderer>().setSprite(currentSprite);
                }            
        }

        public void refreshTextures()
        {
            foreach (var item in frames)
            {
                if (item.sprite != null)
                {
                        item.sprite.SetTexture(AssetPool.TryGetTexture(item.sprite.SpritesheetName));
                }
            }
        }
    }
}
