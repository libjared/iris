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
        public float holdDistance;
        public bool SoftDrop;
        public int deathTimer = 0;
        public float CrosshairCameraRatio;
        public int DropOnDeathCoins;

        public int AMMO_Bullet;
        public int FireTimer = 0;
        public int ReloadTimer = 0;

        public Weapon weapon;

        public List<Weapon> weapons = new List<Weapon>() { };

        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
            Pos = new Vector2f(10, 10);
            Speed = .35f;
            MaxJumps = 2;
            JumpsLeft = MaxJumps;
            Alive = true;
            SetHealth(100);
            idle = new Animation(Content.GetTexture("idle.png"), 4, 120, 1, true);
            running = new Animation(Content.GetTexture("run.png"), 6, 50, 1, true);
            backpedal = new Animation(Content.GetTexture("run.png"), 6, 60, 1, false);
            jumpUp = new Animation(Content.GetTexture("jumpUp.png"), 1, 60, 0, true);
            jumpDown = new Animation(Content.GetTexture("jumpDown.png"), 3, 60, -5, true);
            animation = idle;
            Texture = Content.GetTexture("idle.png");

            weapons.Add(new Revolver());

            weapon = weapons[0];

        }

        public override void Update()
        {
            base.Update();

            //collisionBox = new IntRect(
            UpdateCollisionBox();
            animation.Update();
            handleControls();
            handleAnimationSetting();
            UpdatePosition();
            CheckProjectiles();
            HandleDeath();
            UpdateOnGround();
            dm.Mailman.SendPlayerPosMessage(UID, Pos, Facing, AimAngle);
            UpdateCoinDropAmount();


            ReloadTimer--;
            FireTimer--;

            if (Input.isKeyTap(Keyboard.Key.K))
                SetHealth(0);

            if (Pos.Y > 245)
                SetHealth(0);


            //frameDelta += (float)MainGame.deltaTime.TotalMilliseconds;

        }

        private void UpdateCoinDropAmount()
        {
            DropOnDeathCoins = (int)(dm.clientCoins * .50f);
        }

        public override void UpdateCollisionBox()
        {
            collisionBox.Left = (int)Pos.X - 9;
            collisionBox.Top = (int)Pos.Y - 55;
            collisionBox.Width = 18;
            collisionBox.Height = 55;

            base.UpdateCollisionBox();
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

            Render.renderStates = Actor.shader;
            Texture pistolHand = Content.GetTexture("pistolHand.png");
            Texture revolver = Content.GetTexture("revolver.png");

            if (ouchTimer > 0)
            {
                shader.Shader.SetParameter("ouch", 1f);
            }
            else
            {
                shader.Shader.SetParameter("ouch", 0f);
            }

            shader.Shader.SetParameter("sampler", pistolHand);
            Render.Draw(pistolHand, Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            shader.Shader.SetParameter("sampler", revolver);
            Render.Draw(revolver, Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            shader.Shader.SetParameter("sampler", Texture);
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);
            Render.renderStates = null;

            RectangleShape rect = new RectangleShape();
            rect.Position = new Vector2f((int)Pos.X, (int)Pos.Y);
            rect.Size = new Vector2f(1, 1);
            rect.FillColor = Color.White;
            rect.OutlineColor = Color.White;
            //MainGame.window.Draw(rect); //Draw players collision point





            RectangleShape col = new RectangleShape();
            col.Position = new Vector2f(collisionBox.Left, collisionBox.Top);
            col.Size = new Vector2f(collisionBox.Width, collisionBox.Height);
            col.FillColor = Color.White;
            col.OutlineColor = Color.Red;
            //MainGame.window.Draw(col); //Draw players collision box
        }

        public void HandleDeath()
        {
            if (Alive)
            {
                if (Health <= 0)
                {
                    DropMoney(DropOnDeathCoins);
                    dm.clientCoins -= DropOnDeathCoins;
                    Health = 0;
                    Alive = false;
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

                    MainGame.dm.Players.Remove(this);
                }
            }

        }

        public void handleControls()
        {

            if (Mouse.IsButtonPressed(Mouse.Button.Right))
            {
                CrosshairCameraRatio = 1f;
                dm.PlayerAimSphereRadius = 175f;
            }
            else
            {
                CrosshairCameraRatio = .5f;
                dm.PlayerAimSphereRadius = 100f;
            } //Right click zooming? Kinda cool maybe I dunno

            AimAngle = Helper.angleBetween(MainGame.worldMousePos, Core);

            if (ReloadTimer == 0)
            {
                AMMO_Bullet = 8;
            }

            if (Input.isMouseButtonTap(Mouse.Button.Left))
            {
                if (AMMO_Bullet > 0)
                {
                    if (FireTimer <= 0)
                    {
                        //Console.WriteLine("Bang");
                        Bullet b = new Bullet(this.UID, AimAngle, Core + Helper.PolarToVector2(28, AimAngle, 1, 1));
                        Gui.CrosshairFireExpand = .75f;
                        dm.Projectiles.Add(b);
                        MainGame.Camera.Center += Helper.PolarToVector2(3.5f * MainGame.rand.Next(1, 2), AimAngle + (float)Math.PI, 1, 1);
                        holdDistance = -10f;
                        MainGame.dm.GameObjects.Add(new GunSmoke(Core + Helper.PolarToVector2(32, AimAngle, 1, 1) + (Velocity), AimAngle));
                        MainGame.dm.GameObjects.Add(new GunFlash(Core + Helper.PolarToVector2(32, AimAngle, 1, 1) + (Velocity), AimAngle));
                        AMMO_Bullet--;

                        if (AMMO_Bullet == 0)
                            if (ReloadTimer < 0)
                                ReloadTimer = 70;

                        FireTimer = 10;
                        dm.Mailman.SendBulletCreate(b);
                    }
                }
                else
                {
                    if (ReloadTimer < 0)
                        ReloadTimer = 70;
                }
            }

            if (Input.isKeyTap(Keyboard.Key.W) || Input.isKeyTap(Keyboard.Key.Space))
            {
                if (OnGround || (JumpsLeft > 0)) // && Velocity.Y > 0))
                {
                    TrainDust td = new TrainDust(this.Pos, 0, 1f);
                    td.Alpha = 1;
                    dm.GameObjects.Add(td);
                    JumpsLeft--;
                    Vector2f nextVec = new Vector2f(0, -10f);
                    this.Velocity = nextVec;
                }
            }
            if (Input.isKeyDown(Keyboard.Key.A))
            {
                this.Velocity.X -= 2;
            }
            SoftDrop = Input.isKeyDown(Keyboard.Key.S);
            bool downKey = Input.isKeyTap(Keyboard.Key.S);
            if (downKey)
            {
                Velocity.Y = 10;
            }

            if (Input.isKeyDown(Keyboard.Key.D))
            {
                this.Velocity.X += 2;
            }

            Facing = 1;
            if (Input.screenMousePos.X < MainGame.WindowSize.X / 2)
            {
                Facing = -1;
            }

            if (Input.isKeyDown(Keyboard.Key.E))
            {
                DropMoney(MainGame.rand.Next(8, 16));
            }
            if (Input.isKeyTap(Keyboard.Key.R))
            {
                dm.GameObjects.Add(new TreasureBox(this.Pos));
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
            //increase velocity by gravity, up to a maximum
            Velocity.Y += dm.gravity;
            if (Velocity.Y > dm.gravity * 12)
            {
                Velocity.Y = dm.gravity * 12;
            }

            //horizontal decay, up to a maximum
            Velocity.X *= .75f;
            Velocity.X = Helper.ClampSigned(2f, Velocity.X);

            //truncate tiny velocities
            if (Math.Abs(Velocity.X) < 1) Velocity.X = 0;

            //determine whether we're dropping through platforms
            CollideTypes type = SoftDrop ? CollideTypes.Hard : CollideTypes.HardOrSoft;

            //we do the horizontal and vertical movements separately
            SolveHoriz();
            SolveVert();
        }

        private void UpdateOnGround()
        {
            Vector2i posi = new Vector2i((int)Pos.X, (int)Pos.Y);
            Vector2i below = posi + new Vector2i(0, 1);
            CollideTypes type = SoftDrop ? CollideTypes.Hard : CollideTypes.HardOrSoft;
            OnGround = dm.MapCollide(below.X, below.Y, type);

            //also set jumps. if just walked off a cliff, take away first jump
            if (OnGround)
            {
                JumpsLeft = MaxJumps;
            }
            else if (JumpsLeft == MaxJumps)
            {
                JumpsLeft = MaxJumps - 1;
            }
        }

        private void SolveVert()
        {
            Vector2i posi = new Vector2i((int)Pos.X, (int)Pos.Y);
            Vector2i vertDest = new Vector2i(posi.X, posi.Y + (int)Velocity.Y);

            //if we're to move at all
            if (vertDest.Y != posi.Y)
            {
                int direction = Math.Sign(vertDest.Y - posi.Y);
                CollideTypes type = (SoftDrop || direction == -1) ? CollideTypes.Hard : CollideTypes.HardOrSoft;

                bool hitSomething = true;
                int y = posi.Y;
                while (!dm.MapCollide(posi.X, y, type))
                {
                    if (y == vertDest.Y)
                    {
                        hitSomething = false;
                        break;
                    }
                    y += direction;
                }
                if (hitSomething)
                {
                    Velocity.Y = 0;
                    y -= direction;
                }
                Pos.Y = y;
            }
        }

        private void SolveHoriz()
        {
            Vector2i posi = new Vector2i((int)Pos.X, (int)Pos.Y);
            Vector2i horizDest = new Vector2i(posi.X + (int)Velocity.X, posi.Y);

            //if we're to move at all
            if (horizDest.X != posi.X)
            {
                int direction = Math.Sign(horizDest.X - posi.X);

                bool hitSomething = true;
                int x = posi.X;
                while (!dm.MapCollide(x, posi.Y, CollideTypes.Hard))
                {
                    if (x == horizDest.X)
                    {
                        hitSomething = false;
                        break;
                    }
                    x += direction;
                }
                if (hitSomething)
                {
                    Velocity.X = 0;
                    x -= direction;
                }
                Pos.X = x;
            }
        }

        private void SetHealth(int h)
        {
            Health = h;
            dm.Mailman.SendHealth(Health);
        }

        private void CheckProjectiles()
        {
            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                if (Helper.Distance(MainGame.dm.Projectiles[i].Pos, Core) < 20)
                {
                    ouchTimer = 10;
                    SetHealth(this.Health - MainGame.dm.Projectiles[i].Damage);
                    this.Velocity += MainGame.dm.Projectiles[i].Velocity * 10;
                    MainGame.dm.Projectiles[i].Destroy();
                }
            }
        }

        private void DropMoney(int count)
        {
            MainGame.dm.Mailman.SendCoinCreate(this.Core, count);
        }
    }
}
