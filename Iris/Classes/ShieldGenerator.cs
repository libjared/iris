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

        Texture tex, col;
        private RenderStates shader;
        readonly DateTime whenPlaced;
        readonly TimeSpan lifespan = new TimeSpan(0,0,10); //lasts for 10 sec
        const float radiusStart = 125f;
        const float radiusEnd = 40f;
        float Radius
        {
            get
            {
                var now = DateTime.Now;
                var then = whenPlaced + lifespan;
                var timeSince = now - whenPlaced;
                var weight = timeSince.Ticks / (float)lifespan.Ticks;
                float rad = Helper.Lerp(radiusStart, radiusEnd, weight);
                return rad;
            }
        }
        bool TimesUp
        {
            get
            {
                return DateTime.Now.CompareTo(whenPlaced + lifespan) != -1;
            }
        }


        public ShieldGenerator(Vector2f position)
        {
            tex = Content.GetTexture("generatorStand.png");
            col = Content.GetTexture("genBlue.png");
            shader = new RenderStates(new Shader(null, "Content/shield.frag"));
            whenPlaced = DateTime.Now;
            Pos = position;
        }

        public override void Update()
        {
            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                Projectile p = MainGame.dm.Projectiles[i];
                if (Helper.Distance(p.Pos, this.Pos) < Radius)
                {
                    MainGame.dm.Projectiles[i].Destroy();
                }
            }

            if (TimesUp)
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
            Vector2f centerf = new Vector2f(center.X, MainGame.WindowSize.Y - center.Y);
            float seconds = (float)(DateTime.Now - MainGame.startTime).TotalMilliseconds / 1000f;
            shader.Shader.SetParameter("center", centerf);
            shader.Shader.SetParameter("seconds", seconds);
            Render.DrawCircle(Pos, Radius);
            Render.renderStates = null;
            base.Draw();
        }

    }
}
