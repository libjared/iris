using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Explosion : GameObject
    {
        Animation animation;
        bool damaged = false;

        public Explosion(Vector2f center)
        {
            this.Pos = center;
            animation = new Animation(Content.GetTexture("explosion.png"), 6, 30, 0);
        }

        public override void Update()
        {
            if (!damaged)
            {

                foreach (Actor a in MainGame.dm.Players)
                {
                    if (a is ClientPlayer)
                    {
                        float angleBetween = Helper.angleBetween(this.Pos, a.Core);
                        if (Helper.Distance(a.Pos, this.Pos) < 110)
                        {
                            ((ClientPlayer)a).SetHealth(a.Health - 20);
                            a.ouchTimer = 10;
                            a.Velocity -= new Vector2f((float)Math.Cos(angleBetween) + 3, (float)Math.Sin(angleBetween) + 1) * 5;
                        }
                        if (Helper.Distance(a.Pos, this.Pos) < 90)
                        {
                            ((ClientPlayer)a).SetHealth(a.Health - 30);
                            a.ouchTimer = 10;
                            a.Velocity -= new Vector2f((float)Math.Cos(angleBetween) + 3, (float)Math.Sin(angleBetween) + 1) * 6;
                        }
                        if (Helper.Distance(a.Pos, this.Pos) < 60)
                            ((ClientPlayer)a).SetHealth(0);
                    }
                }
                damaged = true;
                for (int i = 0; i < 6; i++)
                {
                    int gibNum = MainGame.rand.Next(1, 3);
                    Gib g = new Gib(new Texture(Content.GetTexture("explosionGib" + gibNum + ".png")), this.Pos, (float)MainGame.rand.NextDouble() * 6.5f,
                        Helper.angleBetween(this.Pos, this.Pos - new Vector2f(0, 4)) + (float)Math.PI + (float)(i - 5 / 10f) + (float)MainGame.rand.NextDouble());

                    MainGame.dm.GameObjects.Add(g);
                }
            }
            animation.Update();
            if (animation.Frame >= animation.Count - 1)
                MainGame.dm.GameObjects.Remove(this);
            base.Update();
        }

        public override void Draw()
        {
            this.Texture = animation.Texture;
            Render.DrawAnimation(Texture, this.Pos + new Vector2f(-15, 26), Color.White, new Vector2f(Texture.Size.X / (animation.Count * 6),
                Texture.Size.Y - animation.YOffset), 1, animation.Count, animation.Frame, 1);
            //Render.Draw(animation., this.Pos, Color.White, new Vector2f(0, 0), 1, 0);
            base.Draw();
        }
    }
}
