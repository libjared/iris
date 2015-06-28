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
        Animation idle, running, jumpUp, jumpDown, backpedal;
        public Vector2f oldPosition;
        public int animPadding; //Wait a slight bit before changing animations since position may be unreliable

        public NetPlayer(Deathmatch dm, long UID)
            : base(dm)
        {
            this.UID = UID;
            idle = new Animation(Content.GetTexture("idle.png"), 4, 120, 1);
            running = new Animation(Content.GetTexture("run.png"), 6, 60, 2);
            backpedal = new Animation(Content.GetTexture("run.png"), 6, 60, 2, false);
            jumpUp = new Animation(Content.GetTexture("jumpUp.png"), 1, 60, 0);
            jumpDown = new Animation(Content.GetTexture("jumpDown.png"), 3, 60, -5);
            animation = idle;
            Texture = Content.GetTexture("idle.png");
            //.Color = Color.Red;
        }

        public override void Update()
        {
            base.Update();
            animation.Update();
            handleAnimationSetting();
            oldPosition = Pos;
        }

        public override void Draw()
        {
            Core = Pos - new Vector2f(-1, 35);
            this.Texture = animation.Texture;
            Render.Draw(Content.GetTexture("pistolHand.png"), Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.Draw(Content.GetTexture("revolver.png"), Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);
            base.Draw();
        }

        public void handleAnimationSetting()
        {
            OnGround = false;
            if (dm.MapCollide((int)this.Pos.X, (int)this.Pos.Y + (int)dm.gravity))
                OnGround = true;
            animPadding--;
            

            if (animPadding <= -0)
            {
                //if (Helper.Distance(oldPosition,Pos) < 2)
                 animation = idle;
                if (OnGround) //On Ground
                {
                    if (Facing == 1)
                    {
                        if (oldPosition.X - Pos.X < -.2f)
                        {
                            animation = running;
                            animPadding = 5;
                        }
                        if (oldPosition.X - Pos.X > .2f)
                        {
                            animation = backpedal;
                            animPadding = 5;
                        }

                    }
                    if (Facing == -1)
                    {
                        if (oldPosition.X - Pos.X < -.2f)
                        { 
                            animation = running;
                            animPadding = 5;
                        }
                        if (oldPosition.X - Pos.X > .2f)
                        {
                            animation = backpedal;
                            animPadding = 5;
                        }


                    }
                }
                if (!OnGround) // Not on Ground
                {
                    if ((oldPosition.Y - Pos.Y) > 2)
                    {
                        animation = jumpUp;
                        animPadding = 5;
                    }
                    if ((oldPosition.Y - Pos.Y) < 2)
                    {
                        animation = jumpDown;
                        animPadding = 5;
                    }
                }
            }
        }
    }
}
