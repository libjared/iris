using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    class ClientPlayer : Player
    {
        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
            Speed = .35f;
            MaxJumps = 2;
            JumpsLeft = MaxJumps;
        }

        public override void Update()
        {
            dm.Mailman.SendPlayerPosMessage(this.UID, this.Position);
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
                Vector2f nextVec = this.Velocity + new Vector2f(-2, 0);
                this.Velocity = nextVec;
            }
            if (Input.isKeyDown(Keyboard.Key.S))
            {
                //Vector2f nextVec = this.Velocity + new Vector2f(-2, 0);
                //this.Velocity = nextVec;
            }
            if (Input.isKeyDown(Keyboard.Key.D))
            {
                Vector2f nextVec = this.Velocity + new Vector2f(2, 0);
                this.Velocity = nextVec;
            }
        }

        public void updatePosition()
        {
            Velocity += new Vector2f(0, dm.gravity);
            if (dm.MapCollide((int)(Position.X + Velocity.X), (int)Position.Y)) //Right and Left Walls
            {
                Vector2f nextVec = new Vector2f(0, Velocity.Y);
                Velocity = nextVec;
                Color = Color.Green;
            }

            if (dm.MapCollide((int)(Position.X), (int)(Position.Y + Velocity.Y))) //Ground Collision
            {
                OnGround = true;
                JumpsLeft = MaxJumps;
                Vector2f nextVec = new Vector2f(Velocity.X, 0);
                Velocity = nextVec;
                Color = Color.Green;
            }
            else //Not On Ground
            {
                OnGround = false;
                Color = Color.White;
            }

            if (Velocity.Y > dm.gravity * 12)
            {
                Vector2f nextVecA = Velocity;
                nextVecA.Y = dm.gravity * 12;
                Velocity = nextVecA;
            }
            this.Position += Velocity * Speed;
            Vector2f nextVecSlow = new Vector2f(Velocity.X * .75f, Velocity.Y);
            Velocity = nextVecSlow;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
