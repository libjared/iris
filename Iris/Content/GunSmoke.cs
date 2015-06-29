using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class GunSmoke : GameObject
    {
        public Animation animation;

        public GunSmoke() :base()
        {
           // animation = new Animation(Content.GetTexture("smoke"))
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
