using GameEngine.util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.components
{
    public class Animation
    {
        public string Name = "default";
       //  public Dictionary<Sprite,float> frames=new Dictionary<Sprite, float>();
    [JsonRequired]    public List<Frame> frames = new List<Frame>();
        public void AddFrame(Sprite sprite,float frameRate)
        {
            Frame frame = new Frame();
            frame.sprite = sprite;
            frame.frameRate = frameRate;
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
            Frame frame = new Frame();
            frame.sprite = sheet.GetSprite(index);
            frame.frameRate = frameRate;
            frames.Add(frame);
        }
        public bool Loop = true;
        int currentIndex = 0;
        float timeTracker=0;
    [JsonIgnore]    public Sprite currentSprite;
        public void Update(GameObject obj)
        {            
                timeTracker -= Time.deltaTime;
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
