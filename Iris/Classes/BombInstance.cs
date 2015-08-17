using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class BombInstance : Projectile
    {
        int lifeRemaining = 160;

        public BombInstance(long UIDOwner, float angle, Vector2f position)
        {
            this.Angle = angle;
            this.Pos = position;
            this.Speed = 7;
            this.Damage = 25;
            this.Texture = Content.GetTexture("bomb.png");
            this.weightFactor = .020f;
            this.Velocity = Helper.normalize((new Vector2f((float)Math.Cos(Angle), (float)Math.Sin(Angle))));
            this.Owner = MainGame.dm.GetPlayerWithUID(UIDOwner);
        }

        public override void Update()
        {
            lifeRemaining--;
            if (MainGame.dm.MapCollide((int)Pos.X, (int)Pos.Y + (int)Velocity.Y * 2 + 7, CollideTypes.Hard))
                this.Speed = 0;

            if (Velocity.Y > 0 && MainGame.dm.MapCollide((int)Pos.X, (int)Pos.Y + 7, CollideTypes.HardOrSoft))
                this.Speed = 0;

            if (Pos.Y > 215 || lifeRemaining < 0)
            {
                Destroy();
            }

            this.Velocity.Y += weightFactor;
            if (Speed > 0)
                this.Rot += .1f;
            this.Pos += (Velocity * Speed);
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2), 1, Rot, 1);
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
            MainGame.dm.GameObjects.Add(new Explosion(this.Pos, Owner));
            MainGame.dm.GameObjects.Add(new BulletDestroy(this.Pos + new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2) - (Velocity * Speed)));
            MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("explosion.wav"), 1, 0));
            //MainGame.dm.GameObjects.Add(new GunSmoke(this.Pos + new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2) - (Velocity * Speed), 0));
        }
    }
}
