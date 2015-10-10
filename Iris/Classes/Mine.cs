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
    public class Mine : GameObject
    {

        Actor owner;
        Texture tex, shield;
        float size, radius;
        float timer;

        public Mine(Vector2f position, Actor owner)
        {
            this.owner = owner;
            tex = Content.GetTexture("mine.png");
            shield = Content.GetTexture("shield.png");
            this.Pos = position;
            size = .05f;
        }

        public override void Update()
        {
            timer++;
            radius = 20;
            if (timer > 60)
                if (Helper.Distance(MainGame.dm.player.Pos, this.Pos) < radius && MainGame.dm.player.AliveTimer > 120)
                {
                    MainGame.dm.GameObjects.Add(new Explosion(this.Pos - new Vector2f(20, 0), owner));
                    MainGame.dm.GameObjects.Remove(this);
                }
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(tex, this.Pos, Color.White, new Vector2f(9, 6), 1, 0f);
            if (timer > 60)
                Render.Draw(shield, this.Pos, Color.Red, new Vector2f(400, 400), 1, 0, size);
            base.Draw();
        }

    }
}
