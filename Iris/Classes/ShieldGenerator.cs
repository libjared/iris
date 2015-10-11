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
    public class ShieldGenerator : GameObject
    {

        Texture tex, col, shield;
        float size, radius;
        private RenderStates shader;

        public ShieldGenerator(Vector2f position)
        {
            tex = Content.GetTexture("generatorStand.png");
            col = Content.GetTexture("genBlue.png");
            shield = Content.GetTexture("shield.png");
            var sh = new Shader(null, "Content/shield.frag");
            shader = new RenderStates(sh);
            this.Pos = position;
            size = .35f;
        }

        public override void Update()
        {
            radius = size * .45f * shield.Size.X;
            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                Projectile p = MainGame.dm.Projectiles[i];
                if (Helper.Distance(p.Pos, this.Pos) < (radius))
                {
                    MainGame.dm.Projectiles[i].Destroy();
                }
            }
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
            
            Render.renderStates = shader;
            Vector2i center = MainGame.window.MapCoordsToPixel(Pos);
            Vector2f centerf = new Vector2f(1f, 1f) - new Vector2f(center.X / (float)MainGame.WindowSize.X, center.Y / (float)MainGame.WindowSize.Y);
            Console.WriteLine(centerf);
            shader.Shader.SetParameter("center", centerf);
            shader.Shader.SetParameter("seconds", (float)(DateTime.Now - MainGame.startTime).TotalMilliseconds/1000f);
            Render.Draw(shield, this.Pos, Color.White, new Vector2f(400, 400), 1, 0, size);
            Render.renderStates = null;
            base.Draw();
        }

    }
}
