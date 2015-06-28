using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Iris
{
    public class Animation
    {
        public Texture texture;
        public int frame;
        public int count;
        public int duration;

        private float timer;
        

        public Animation(Texture t, int count)
        {
            this.count = count;
            this.texture = t;
        }

        public void Update()
        {
            timer += MainGame.startTime.Millisecond;

            if (timer > duration)
            {
                frame++;
                if (frame >= count)
                    frame = 0;
                timer = 0;
            }
        }

    }


}
