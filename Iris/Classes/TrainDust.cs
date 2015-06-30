using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class TrainDust : GameObject
    {
        public Animation animation;

        public TrainDust(Vector2f pos, float Rotation)
            : base()
        {
            this.Pos = pos;
            this.Rot = Rotation;
            animation = new Animation(Content.GetTexture("gunSmoke.png"), 5, 25, 0);
        }

        public override void Update()
        {
            this.Pos.X -= 2f;
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
            //new Color(224,164,145, 200) //brownish
            Render.DrawAnimation(Texture, this.Pos, new Color(255,255,255,100), new Vector2f(16, 8), 1, animation.Count, animation.Frame, 1, Rot);

            base.Draw();
        }
    }
}
