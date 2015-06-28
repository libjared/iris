using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Iris
{
    class NetPlayer : Actor
    {
        Animation idle, running, jumpUp, jumpDown;

        public NetPlayer(Deathmatch dm, long UID)
            : base(dm)
        {
            this.UID = UID;
            idle = new Animation(Content.GetTexture("idle.png"), 4, 120, 1);
            running = new Animation(Content.GetTexture("run.png"), 6, 60, 2);
            jumpUp = new Animation(Content.GetTexture("jumpUp.png"), 1, 60, 0);
            jumpDown = new Animation(Content.GetTexture("jumpDown.png"), 3, 60, 0);
            animation = idle;
            Texture = Content.GetTexture("idle.png");
            //.Color = Color.Red;
        }

        public override void Update()
        {
            base.Update();
            animation.Update();
        }

        public override void Draw()
        {
            this.Texture = animation.Texture;
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);
            base.Draw();
        }

        public void handleAnimationSetting()
        {
            if (OnGround) //On Ground
            {
                if (Math.Abs(Velocity.X) > .2f)
                    animation = running;
                else
                    animation = idle;
                if (Math.Abs(Velocity.X) == 2)
                    animation = idle;
            }
            if (!OnGround) // Not on Ground
            {
                if (Velocity.Y < 2)
                {
                    animation = jumpUp;
                }
                if (Velocity.Y > 2)
                {
                    animation = jumpDown;
                }
            }
        }
    }
}
