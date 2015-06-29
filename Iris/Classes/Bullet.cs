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
        public Bullet(long UIDOwner, float angle, Vector2f position, float speed, int damage)
        {
            this.Angle = angle;
            this.Pos = position;
            this.Speed = speed;
            this.Damage = damage;
            this.Texture = Content.GetTexture("bullet.png");
        }

        public override void Update()
        {
            if (this.Pos.X < 0 || this.Pos.X > 5000)
            {
                MainGame.dm.Projectiles.Remove(this);
            }

            Velocity = Helper.normalize((new Vector2f((float)Math.Cos(Angle), (float)Math.Sin(Angle))));
            this.Rot = Helper.angleBetween(this.Pos, this.Pos + Velocity);
            this.Pos += (Velocity * Speed);
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X/2, Texture.Size.Y/2), 1, Rot, 1);
            base.Draw();
        }

        public override void onPlayerHit(Actor hit)
        {
            MainGame.dm.Projectiles.Remove(this);
        }
    }
}
