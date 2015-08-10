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
        public int weaponIndex = 0;

        public NetPlayer(Deathmatch dm, long UID)
            : base(dm)
        {
            this.UID = UID;
            idle = new Animation(Content.GetTexture(model.idleFile), 4, 120, 1);
            running = new Animation(Content.GetTexture(model.runFile), 6, 60, 2);
            backpedal = new Animation(Content.GetTexture(model.runFile), 6, 60, 2, false);
            jumpUp = new Animation(Content.GetTexture(model.jumpUpFile), 1, 60, 0);
            jumpDown = new Animation(Content.GetTexture(model.jumpDownFile), 3, 60, -5);
            animation = idle;

            Texture = Content.GetTexture("idle.png");
            Alive = true;
            //.Color = Color.Red;

            weapons = new List<Weapon>()
            {
                new Revolver(this),
                new Shotgun(this),
                new MachineGun(this),
                new BombWeapon(this),
            };
        }

        public override void Update()
        {
            HandleDeath();
            base.Update();


            if (!Alive)
                return;
            this.weapon = weapons[weaponIndex];
            animation.Update();
            HandleAnimationSetting();
            CheckProjectiles();

            oldPosition = Pos;
        }

        public override void Draw()
        {
            if (!Alive)
                return;

            Core = Pos - new Vector2f(-1, 35);
            this.Texture = animation.Texture;
            Render.renderStates = Actor.shader;
            Texture pistolHand = Content.GetTexture("pistolHand.png");
            Texture weaponTexture = weapon.texture;

            if (ouchTimer > 0)
            {
                shader.Shader.SetParameter("ouch", 1f);
            }
            else
            {
                shader.Shader.SetParameter("ouch", 0f);
            }

            shader.Shader.SetParameter("sampler", pistolHand);
            Render.Draw(pistolHand, Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            shader.Shader.SetParameter("sampler", weaponTexture);
            Render.Draw(weaponTexture, Core, Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);
            shader.Shader.SetParameter("sampler", Texture);
            Render.DrawAnimation(Texture, this.Pos, Color.White, new Vector2f(Texture.Size.X / (animation.Count * 4),
                Texture.Size.Y - animation.YOffset), Facing, animation.Count, animation.Frame, 1);
            Render.renderStates = null;

            Render.DrawString(Content.GetFont("Font1.ttf"), Name, Core - new Vector2f(0, 40), Color.White, .3f, true, 1);
            base.Draw();
        }

        public void CheckProjectiles()
        {
            for (int i = 0; i < MainGame.dm.Projectiles.Count; i++)
            {
                if (Helper.Distance(MainGame.dm.Projectiles[i].Pos, Core) < 20)
                {
                    this.Killer = MainGame.dm.Projectiles[i].Owner;
                    MainGame.dm.Projectiles[i].Destroy();
                    ouchTimer = 10;
                }
            }
        }

        public override void UpdateCollisionBox()
        {
            collisionBox.Left = (int)Pos.X - 9;
            collisionBox.Top = (int)Pos.Y - 55;
            collisionBox.Width = 18;
            collisionBox.Height = 55;

            base.UpdateCollisionBox();
        }

        public void HandleDeath()
        {
            if (Alive)
            {
                if (Health <= 0)
                {
                    Alive = false;
                    for (int i = 0; i < MainGame.gibCount; i++)
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

        public void HandleAnimationSetting()
        {
            OnGround = false;
            if (dm.MapCollide((int)this.Pos.X, (int)this.Pos.Y + 1, CollideTypes.HardOrSoft))
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
            hit.Destroy();
            //Probably wont do anything
            base.OnProjectileHit(hit);
        }
    }
}
