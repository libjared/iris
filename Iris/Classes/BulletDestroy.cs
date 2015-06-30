using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class BulletDestroy : GameObject
    {
        public Animation animation;

        public BulletDestroy(Vector2f pos)
            : base()
        {
            this.Pos = pos;
            animation = new Animation(Content.GetTexture("bulletDestroy.png"), 4, 10, 0);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            animation.Update();
            this.Texture = animation.Texture;
            if (animation.Frame >= animation.Count - 1)
            {
                MainGame.dm.GameObjects.Remove(this);
            }
            Render.DrawAnimation(Texture, this.Pos, new Color(255,255,255,200), new Vector2f(16, 8), 1, animation.Count, animation.Frame, 1, Rot);

            base.Draw();
        }
    }
}
