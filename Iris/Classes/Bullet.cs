using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Bullet : Projectile
    {
        public Bullet(float angle, Vector2f position, float speed, int damage)
        {
            this.Angle = angle;
            this.Pos = position;
            this.Speed = speed;
            this.Damage = damage;
            Texture = Content.GetTexture("flint_right.png");

            this.Position = Pos;
            this.Rotation = Rot;
        }

        public override void Update()
        {
            Rot = Helper.angleBetween(this.Pos, this.Pos + Velocity);
            Velocity = Helper.normalize((new Vector2f((float)Math.Cos(Angle), (float)Math.Sin(Angle))));

            this.Pos += Velocity * Speed;
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
