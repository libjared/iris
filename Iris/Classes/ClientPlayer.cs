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
        Animation idle, running, jumpUp, jumpDown;

        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
            Pos = new Vector2f(10, 10);
            Speed = .35f;
            MaxJumps = 200;
            JumpsLeft = MaxJumps;

            idle = new Animation(new Sprite(Content.GetTexture("idle.png")), 4, 120);
            animation = idle;
            Sprite.Texture = animation.getTexture();
        }

        public override void Update()
        {
            base.Update();
            handleControls();
            UpdatePosition();
            dm.Mailman.SendPlayerPosMessage(UID, Pos);

            
        }

        public void handleControls()
        {
            //Console.WriteLine(Helper.angleBetween(MainGame.worldMousePos, Pos));
            if (Input.isMouseButtonTap(Mouse.Button.Left))
            {
                //Console.WriteLine("Bang");
                dm.Projectiles.Add(new Bullet(Helper.angleBetween(MainGame.worldMousePos, Pos), Pos, 1, 0));
            }

            if (Input.isKeyTap(Keyboard.Key.W))
            {
                if (OnGround || JumpsLeft > 0)
                {
                    JumpsLeft--;
                    Vector2f nextVec = new Vector2f(0, -24f);
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
        }

        private void UpdatePosition()
        {
            Sprite.Color = Color.White;
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
            //if destination is all clear, just set Pos and we're done //Nope, dont do this
            //Vector2f dest = Velocity + Pos;
            //if (!dm.MapCollide((int)dest.X, (int)dest.Y))
            //{
            //    Pos = dest;
            //    return;
            //}
            
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

        public override void Draw()
        {
            base.Draw();
            RectangleShape rect = new RectangleShape();
            rect.Position = new Vector2f((int)Pos.X, (int)Pos.Y);
            rect.Size = new Vector2f(1, 1);
            rect.FillColor = Color.White;
            rect.OutlineColor = Color.White;
            MainGame.window.Draw(rect);
        }
    }
}
