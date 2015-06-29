using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Gib : GameObject
    {
        public float speed;
        public float angle;
        public Vector2f velocity;
        public float fallVel;
        public bool rotDir; //Rotation Direction = true / clockwise

        public Gib(Texture texture, Vector2f pos, float speed, float angle)
            : base()
        {
                this.Texture = texture;
            this.Pos = pos;
            this.angle = angle;
            Rot = (float)MainGame.rand.NextDouble() - .5f;
            this.speed = speed;
            if (MainGame.rand.Next(100) < 50)
                rotDir = true;
        }

        public override void Update()
        {
            if (speed != 0) //Not yet at rest
            {
                fallVel += .1f;
                Rot += .05f * speed * (rotDir ? 1 : -1);
            }
            velocity = Helper.normalize(new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle)));
            Pos += velocity * speed;
            Pos.Y += fallVel;

            if (MainGame.dm.MapCollide((int)Pos.X, (int)(Pos.Y + fallVel / 2)))
            {
                speed = 0;
                fallVel = 0;
            }
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(Texture, Pos, Color.White, new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2), 1, Rot, 1);
            base.Draw();
        }
    }
}
