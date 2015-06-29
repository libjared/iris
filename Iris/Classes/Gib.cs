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
        public float fallOff;

        public Gib(Texture texture, Vector2f pos, float speed, float angle)
            : base()
        {
            this.Texture = texture;
            fallOff = .6f;
            this.angle = angle;
            Rot = (float)MainGame.rand.NextDouble();
            this.speed = (float)MainGame.rand.NextDouble() * speed;

            velocity = Helper.normalize(new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle))) * speed;
        }

        public override void Update()
        {
            velocity = Helper.normalize(new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle)));
            Pos += velocity * speed;
            base.Update();
        }

        public override void Draw()
        {
            //Render.Draw(Texture, )
            base.Draw();
        }
    }
}
