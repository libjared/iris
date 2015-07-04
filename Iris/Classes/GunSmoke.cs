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

        public GunSmoke(Vector2f pos, float Rotation) :base()
        {
            this.Alpha = .8f;
            this.Pos = pos;
            this.Rot = Rotation;
            animation = new Animation(Content.GetTexture("gunSmoke.png"), 5, 15, 0);
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
            Render.DrawAnimation(Texture, this.Pos, new Color(255,255,255, (byte)(255 * Alpha)), new Vector2f(16,8), 1, animation.Count, animation.Frame, 1, Rot);

            base.Draw();
        }
    }
}
