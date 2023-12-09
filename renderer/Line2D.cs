using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.renderer
{
    public class Line2D
    {
        private Vector2 from;
        private Vector2 to;
        private Vector3 color;
        private int lifetime;

        public Line2D(Vector2 from, Vector2 to, Vector3 color, int lifetime)
        {
            this.from = from;
            this.to = to;
            this.color = color;
            this.lifetime = lifetime;
        }

        public Vector2 getFrom()
        {
            return from;
        }
        public Vector2 getto()
        {
            return to;
        }
        public Vector3 getColor()
        {
            return color;
        }
        public int getLifetime()
        {
            return lifetime;    
        }

        public int beginFrame()
        {
            this.lifetime--;
            return lifetime;
        }
    }
}