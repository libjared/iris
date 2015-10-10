using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iris
{
    public class HealthGenerator : GameObject
    {

        Texture tex, col, shield;
        float size, radius;
        float timer;

        public HealthGenerator(Vector2f position)
        {
            tex = Content.GetTexture("generatorStand.png");
            col = Content.GetTexture("genGreen.png");
            shield = Content.GetTexture("shield.png");
            this.Pos = position;
            size = .35f;
        }

        public override void Update()
        {
            timer++;
            radius = size * .45f * shield.Size.X;
            if (Helper.Distance(MainGame.dm.player.Pos, this.Pos) < radius)
            {
                if (timer % 17 == 0)
                {
                    //if (MainGame.dm.player.Health < 100)
                    MainGame.dm.player.Health += 1;
                }
            }

            //for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            //{
            //    Projectile p = MainGame.dm.Projectiles[i];
            //    if (Helper.Distance(p.Pos, this.Pos) < (radius))
            //    {
            //        MainGame.dm.Projectiles[i].Destroy();
            //    }
            //}
            size -= .00025f;

            if (size <= 0)
            {
                MainGame.dm.GameObjects.Add(new Explosion(this.Pos - new Vector2f(20, 0), MainGame.dm.player, true));
                MainGame.dm.GameObjects.Remove(this);
            }


            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(tex, this.Pos, Color.White, new Vector2f(10, 31), 1, 0f);
            Render.Draw(col, this.Pos, Color.White, new Vector2f(10, 31), 1, 0f);
            Render.Draw(shield, this.Pos, new Color(0,240,0), new Vector2f(400, 400), 1, 0, size);
            base.Draw();
        }

    }
}
