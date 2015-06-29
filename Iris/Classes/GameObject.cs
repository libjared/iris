using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class GameObject
    {
        public Texture Texture;
        public float Rot;
        public Vector2f Pos;
        public float Alpha = 1;

        public GameObject()
        {
        }

        public virtual void Update()
        { 
            //Game Object on tracks
            if (Pos.Y > 212)
                Pos.Y = 212;
            if (Pos.Y > 209)
                Pos.X -= 15f;
        }

        public virtual void Draw()
        {

        }
    }
}
