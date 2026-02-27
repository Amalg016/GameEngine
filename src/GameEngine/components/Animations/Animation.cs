using GameEngine.Core.Utilities;
using GameEngine.ECS;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GameEngine.components
{
    public class Animation
    {
        public string Name = "default";
        [JsonRequired] public List<Frame> frames = new List<Frame>();
        [JsonRequired] float speed = 1;
        public bool Loop = true;
        int currentIndex = 0;
        float timeTracker = 0;
        [JsonIgnore] public Sprite currentSprite;

        public void AddFrame(Sprite sprite, float frameRate)
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

        public void Play()
        {
            currentIndex = 0;
            timeTracker = 0;
            currentSprite = null;
        }

        public void Update(GameObject obj)
        {
            if (frames.Count == 0 || speed == 0) return;

            timeTracker -= Time.deltaTime * speed;
            if (timeTracker <= 0)
            {
                if (currentSprite == null)
                {
                    // first frame, don't advance the index
                }
                else
                {
                    if (currentIndex < frames.Count - 1)
                    {
                        currentIndex++;
                    }
                    else if (Loop)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        return; // Reached end, do nothing
                    }
                }

                var s = frames[currentIndex];
                timeTracker = s.frameRate;
                currentSprite = s.sprite;
                
                var renderer = obj.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.setSprite(currentSprite);
                }
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
