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
            Alive = true;
            //.Color = Color.Red;
        }

        public override void Update()
        {
            base.Update();
            animation.Update();
            handleAnimationSetting();
            oldPosition = Pos;
            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                if (Helper.Distance(MainGame.dm.Projectiles[i].Pos, Core) < 20)
                {
                    MainGame.dm.Projectiles.RemoveAt(i);
                }
            }
            if (Alive)
            {
                if (Health <= 0)
                {
                    MainGame.dm.Players.Remove(this);
                    Alive = false;
                    //MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibHead.png")), Core, 0,0));
                    for (int i = 0; i < 100; i++)
                    {
                        int gibNum = MainGame.rand.Next(1, 4);
                        Gib g = new Gib(new Texture(Content.GetTexture("gib" + gibNum + ".png")), Core - new Vector2f(0, 4) +
                            new Vector2f(MainGame.rand.Next(-4, 5), MainGame.rand.Next(-4, 5)), (float)MainGame.rand.NextDouble() * 4.5f,
                            Helper.angleBetween(Core, Core - new Vector2f(0, 4)) + (float)Math.PI + (float)(i - 5 / 10f) + (float)MainGame.rand.NextDouble());
                        g.addVel = new Vector2f(Velocity.X / 15, Velocity.Y / 35); //Trauma
                        MainGame.dm.GameObjects.Add(g);
                    }

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibHead.png")), Core - new Vector2f(0, 4), 3,
                        Helper.angleBetween(Core, Core - new Vector2f(0, 4)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibBody.png")), Core - new Vector2f(0, 1), 2,
                        Helper.angleBetween(Core, Core - new Vector2f(1, 2)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibUpperLeg.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(.5f, 1)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibUpperLeg.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(-.5f, 1)) + (float)Math.PI));


                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibLowerLeg.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(.15f, 3)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibLowerLeg.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(-.20f, 2)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibArm.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(.04f, 3)) + (float)Math.PI));

                    MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibArm.png")), Core + new Vector2f(0, 1), 3.2f,
                        Helper.angleBetween(Core, Core - new Vector2f(-.55f, 2)) + (float)Math.PI));
                }
            }
        }

        public override void Draw()
        {
            Core = Pos - new Vector2f(-1, 35);
            this.Texture = animation.Texture;
            Render.Draw(Content.GetTexture("pistolHand.png"), Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.Draw(Content.GetTexture("revolver.png"), Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);

            Render.DrawString(Content.GetFont("Font1.ttf"), Name, Core - new Vector2f(0, 40), Color.White, .3f, true, 1); 
            base.Draw();
        }

        public void handleAnimationSetting()
        {
            OnGround = false;
            if (dm.MapCollide((int)this.Pos.X, (int)this.Pos.Y + (int)dm.gravity, Deathmatch.anyCol))
                OnGround = true;
            animPadding--;
            

            if (animPadding <= -5)
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
                            animation = backpedal;
                            animPadding = 5;
                        }
                        if (oldPosition.X - Pos.X > .2f)
                        {
                            animation = running;
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

        public override void OnProjectileHit(Projectile hit)
        {
            MainGame.dm.Projectiles.Remove(hit);
            //Probably wont do anything
            base.OnProjectileHit(hit);
        }
    }
}
