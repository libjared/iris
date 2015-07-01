using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class CliffFace : GameObject
    {
        public CliffFace()
        {
            this.Texture = Content.GetTexture("CliffOpenBlack.png");
            this.Pos.X = 3500;
        }

        public override void Update()
        {
            this.Pos.Y = -220;
            this.Pos.X -= 15;
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(this.Texture, this.Pos, Color.White, new Vector2f(0, 0), 1, 0f, 1);
            base.Draw();
        }
    }
}
