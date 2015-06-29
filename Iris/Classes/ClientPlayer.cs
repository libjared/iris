using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class ClientPlayer : Actor
    {
        Animation idle, running, jumpUp, jumpDown, backpedal;
        float holdDistance;

        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
            Pos = new Vector2f(10, 10);
            Speed = .35f;
            MaxJumps = 2;
            JumpsLeft = MaxJumps;
            Alive = true;

            idle = new Animation(Content.GetTexture("idle.png"), 4, 120, 1, true);
            running = new Animation(Content.GetTexture("run.png"), 6, 60, 3, true);
            backpedal = new Animation(Content.GetTexture("run.png"), 6, 60, 2, false);
            jumpUp = new Animation(Content.GetTexture("jumpUp.png"), 1, 60, 0, true);
            jumpDown = new Animation(Content.GetTexture("jumpDown.png"), 3, 60, -5, true);
            animation = idle;
            Texture = Content.GetTexture("idle.png");
        }

        public override void Update()
        {
            base.Update();
            animation.Update();
            handleControls();
            handleAnimationSetting();
            UpdatePosition();
            dm.Mailman.SendPlayerPosMessage(UID, Pos, Facing, AimAngle);

            if (Input.isKeyTap(Keyboard.Key.K))
                Health = 0;

            if (Alive)
            {
                if (Health <= 0)
                {
                    MainGame.dm.Players.Remove(this);
                    Alive = false;
                    //MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gibHead.png")), Core, 0,0));
                    for (int i = 0; i < 350; i++ )
                    {
                        int gibNum = MainGame.rand.Next(1, 4);
                        MainGame.dm.GameObjects.Add(new Gib(new Texture(Content.GetTexture("gib"+gibNum+".png")), Core - new Vector2f(0, 4), (float)MainGame.rand.NextDouble() * 5,
                            Helper.angleBetween(Core, Core - new Vector2f(0, 4)) + (float)Math.PI + (float)(i - 5 / 10f) + (float)MainGame.rand.NextDouble()));
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

            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                if (Helper.Distance(MainGame.dm.Projectiles[i].Pos, Core) < 20)
                {
                    this.Health -= MainGame.dm.Projectiles[i].Damage;
                    MainGame.dm.Projectiles.RemoveAt(i);
                }
            }
            //frameDelta += (float)MainGame.deltaTime.TotalMilliseconds;

        }

        public override void Draw()
        {
            base.Draw();

            //if (frame == 1)
            //Console.Write(frame);
            //Render.Draw(animation.Texture, this.Pos, Color.White, new Vector2f(0,0), 1, 0,1);
            if (holdDistance < 0)
                holdDistance += -holdDistance * .05f;
            Core = Pos - new Vector2f(-1, 35);
            this.Texture = animation.Texture;
            Render.Draw(Content.GetTexture("pistolHand.png"), Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.Draw(Content.GetTexture("revolver.png"), Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);
            //Sprite s = new Sprite(idleTest);

            //s.TextureRect = new IntRect(
            //    64 * frame,
            //    0,
            //    64,
            //    55
            //);

            //MainGame.window.Draw(s);
            //Render.Draw(s.Texture, this.Pos, Color.White, new Vector2f(0, 0), 1, 0, 1);

            RectangleShape rect = new RectangleShape();
            rect.Position = new Vector2f((int)Pos.X, (int)Pos.Y);
            rect.Size = new Vector2f(1, 1);
            rect.FillColor = Color.White;
            rect.OutlineColor = Color.White;
            MainGame.window.Draw(rect);
        }

        public void handleControls()
        {

            AimAngle = Helper.angleBetween(MainGame.worldMousePos, Core);
            if (Input.isMouseButtonTap(Mouse.Button.Left))
            {
                //Console.WriteLine("Bang");
                Bullet b = new Bullet(this.UID, AimAngle, Core + Helper.PolarToVector2(28, AimAngle, 1, 1), 6, 40);
                dm.Projectiles.Add(b);
                MainGame.Camera.Center += Helper.PolarToVector2(3.5f * MainGame.rand.Next(1, 2), AimAngle + (float)Math.PI, 1, 1);
                holdDistance = -10f;
                dm.Mailman.SendBulletCreate(b);
            }

            if (Input.isKeyTap(Keyboard.Key.W))
            {
                if (OnGround || (JumpsLeft > 0 && Velocity.Y > .85f))
                {
                    JumpsLeft--;
                    Vector2f nextVec = new Vector2f(0, -24f - JumpsLeft);
                    this.Velocity = nextVec;
                }
            }
            if (Input.isKeyDown(Keyboard.Key.A))
            {
                this.Velocity.X -= 2;
            }
            if (Input.isKeyDown(Keyboard.Key.S))
            {
            }
            if (Input.isKeyDown(Keyboard.Key.D))
            {
                this.Velocity.X += 2;
            }

            Facing = 1;
            if (MainGame.worldMousePos.X < this.Pos.X)
            {
                Facing = -1;
            }
        }

        public void handleAnimationSetting()
        {

            if (OnGround) //On Ground
            {
                animation = idle;
                if (Facing == 1)
                {
                    if (Velocity.X > .2f)
                        animation = running;
                    if (Velocity.X < -.2f)
                        animation = backpedal;
                }
                if (Facing == -1)
                {
                    if (Velocity.X < -.2f)
                        animation = running;
                    if (Velocity.X > .2f)
                        animation = backpedal;
                }

                if (Math.Abs(Velocity.X) == 2)
                    animation = idle;
            }
            if (!OnGround) // Not on Ground
            {
                holdDistance = -3;
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

        public override void OnProjectileHit(Projectile hit)
        {
            this.Health -= hit.Damage;
        }

        private void UpdatePosition()
        {
            //Console.WriteLine(OnGround);
            //Sprite.Color = Color.White;
            //increase velocity by gravity, up to a maximum
            Velocity.Y += dm.gravity;
            if (Velocity.Y > dm.gravity * 15)
            {
                Velocity.Y = dm.gravity * 15;
            }

            if (dm.MapCollide((int)Pos.X, (int)Pos.Y + (int)Velocity.Y))
            {
                JumpsLeft = MaxJumps;
            }
            //if destination is all clear, just set Pos and we're done //Nope, dont do this.
            Vector2f dest = Velocity + Pos;
            OnGround = true;
            if (!dm.MapCollide((int)dest.X, (int)dest.Y))
            {
                OnGround = false;
                // return;
            }

            //Color = Color.Green;

            //we do the horizontal and vertical separately. one of them is blocking us

            //try to clear horizontal
            SolveHoriz();

            //try to clear vertical
            SolveVert();

            Velocity.X *= .75f;
            Pos += Velocity * Speed;
            //horizontal decay

        }

        private void SolveVert()
        {
            Vector2i posi = new Vector2i((int)Pos.X, (int)Pos.Y);
            Vector2i vertDest = new Vector2i(posi.X, posi.Y + (int)Velocity.Y);
            if (!dm.MapCollide(vertDest.X, vertDest.Y))
            {
                //Pos = new Vector2f(vertDest.X, vertDest.Y);
            }
            else //can't, we must search for the wall
            {
                int direction = Math.Sign(vertDest.Y - posi.Y);

                if (direction == 0)
                {
                    Console.WriteLine("stuck in vertical!");
                    return;
                }

                int y = vertDest.Y;
                while (true)
                {
                    y -= direction;
                    if (!dm.MapCollide(posi.X, y))
                    {
                        break;
                    }
                }
                Velocity.Y = 0;
                Pos.Y = y;
            }
        }

        private void SolveHoriz()
        {
            Vector2i posi = new Vector2i((int)Pos.X, (int)Pos.Y);
            Vector2i horizDest = new Vector2i(posi.X + (int)Velocity.X, posi.Y);
            if (!dm.MapCollide(horizDest.X, horizDest.Y))
            {
                //Pos = new Vector2f(horizDest.X, horizDest.Y);
            }
            else //can't, we must search for the wall
            {
                int direction = Math.Sign(horizDest.X - posi.X);

                if (direction == 0)
                {
                    Console.WriteLine("stuck in horizontal!");
                    return;
                }

                int x = horizDest.X;
                while (true)
                {
                    x -= direction;
                    if (!dm.MapCollide(x, posi.Y))
                    {
                        break;
                    }
                }
                Velocity.X = 0;
                Pos.X = x;
            }
        }
    }
}
