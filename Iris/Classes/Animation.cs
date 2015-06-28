using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Animation
    {
        public Texture Texture;
        public int Frame;
        public int Count;
        public int Duration;
        public int YOffset = 0;
        public bool Forward;

        private float timer;


        public Animation(Texture t, int count, int duration, int yOffset, bool forward = true)
        {
            this.Count = count;
            this.Texture = t;
            this.Duration = duration * 100;
            this.YOffset = yOffset;
            this.Forward = forward;
        }

        public void Update()
        {
            timer += 700f;// (float)MainGame.deltaTime.TotalMinutes;

            //Console.WriteLine(frame);
            if (timer > Duration)
            {
                if (Forward)
                {
                    Frame++;
                    if (Frame >= Count)
                        Frame = 0;
                }
                else
                {
                    Frame--;
                    if (Frame < 0)
                        Frame = Count - 1;
                }

                

                    
                
                timer = 0;
            }
            //Console.WriteLine(intRect.Width);

            //sprite.TextureRect = new IntRect(0, 0, 64, 64);//intRect;
            //sprite.Origin = new Vector2f(sprite.Texture.Size.X / 2 / count, sprite.Texture.Size.Y);
        }

        public void Display()
        {

        }
    }
}
