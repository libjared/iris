using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class ClientPlayer : Player
    {
        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
            Pos = new Vector2f(10, 10);
            Speed = .35f;
            MaxJumps = 2;
            JumpsLeft = MaxJumps;
        }

        public override void Update()
        {
            dm.Mailman.SendPlayerPosMessage(this.UID, this.Pos);
            handleControls();
            updatePosition();           
            base.Update();
        }

        public void handleControls()
        {
            if (Input.isKeyTap(Keyboard.Key.W))
            {
                if (OnGround || JumpsLeft > 0)
                {
                    JumpsLeft--;
                    Vector2f nextVec = new Vector2f(0, -15f);
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

        public void updatePosition()
        {
            Velocity.Y += dm.gravity;
            if (Velocity.Y > dm.gravity * 12)
            {
                Velocity.Y = dm.gravity * 12;// = nextVecA;
            }
            
            if (dm.MapCollide((int)(Pos.X + Velocity.X), (int)Pos.Y)) //Right and Left Walls
            {
                Velocity.X = 0;
                Color = Color.Green;
            }
            
            if (dm.MapCollide((int)(Pos.X), (int)(Pos.Y + Velocity.Y))) //Ground Collision
            {
                Velocity.Y = 0;
                OnGround = true;
                JumpsLeft = MaxJumps;
                Color = Color.Green;

                for (float i = 0; i < 10; i+=.01f)
                {
                    if (dm.MapCollide((int)Pos.X, (int)(Pos.Y + i)))
                    {
                        Console.WriteLine(i);
                        Pos.Y += (int)i;
                        break;
                    }
                }
                
            }
            else //Not On Ground
            {
                
                OnGround = false;
                Color = Color.White;
            }
            
            Pos += Velocity * Speed;
            Velocity.X *= .75f;
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
