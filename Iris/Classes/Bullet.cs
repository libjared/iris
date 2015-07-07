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
        int lifeRemaining = 500;

        public Bullet(long UIDOwner, float angle, Vector2f position, int damage)
        {
            this.Angle = angle;
            this.Pos = position;
            this.Speed = 8;
            this.Damage = damage;
            this.Texture = Content.GetTexture("bullet.png");
            this.weightFactor = .0055f;
            this.Velocity = Helper.normalize((new Vector2f((float)Math.Cos(Angle), (float)Math.Sin(Angle))));
        }

        public override void Update()
        {
            lifeRemaining--;
            if (lifeRemaining < 0 || MainGame.dm.MapCollide((int)Pos.X,(int)Pos.Y, CollideTypes.Hard) || Pos.Y > 215)
            {
                Destroy();
            }

            this.Velocity.Y += weightFactor;
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
            Destroy();
            //MainGame.dm.Projectiles.Remove(this);
        }

        public override void Destroy()
        {
            MainGame.dm.Projectiles.Remove(this);
            MainGame.dm.GameObjects.Add(new BulletDestroy(this.Pos + new Vector2f(Texture.Size.X/2, Texture.Size.Y/2) - (Velocity * Speed)));
            //MainGame.dm.GameObjects.Add(new GunSmoke(this.Pos + new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2) - (Velocity * Speed), 0));
        }
    }
}
