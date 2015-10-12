using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Coin : GameObject
    {
        public float speed;
        public float angle;
        public Vector2f velocity;
        public float fallVel;
        public bool rotDir; //Rotation Direction = true / clockwise
        public int lifeRemaining = 500;
        public Vector2f addVel;
        public float pickupLerpPercent = 0;
        public bool pickedUp = false;
        public Actor pickerUpper;
        public int readyTimer = 45; //After maybe half a second they'll be available to be picked up
        public Color c = Color.White;

        public Coin(Vector2f pos, float speed, float angle)
            : base()
        {
            this.Texture = Content.GetTexture("coin.png");
            //if (MainGame.rand.Next(100) < 10)
            //{
            //    this.Texture = Content.GetTexture("diamond.png");
            //    if (MainGame.rand.Next(3) == 0)
            //    {
            //        c = Color.Red;
            //    }
            //    else if (MainGame.rand.Next(3) == 0)
            //    {
            //        c = Color.Green;
            //    }
            //    else
            //    {
            //        c = Color.Cyan;
            //    }
            //}
            this.Pos = pos;
            this.angle = angle;
            this.speed = speed;

            
            //MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("coin.wav"), .4f, .4f));
            velocity = speed * Helper.normalize(new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle))) + addVel;
        }

        public override void Update()
        {
            //Game Object on tracks
            CheckCollect();
            readyTimer--;
            if (Pos.Y > 212)
                Pos.Y = 212;
            if (Pos.Y > 209)
                Pos.X -= 15f;

            lifeRemaining--;

            if (lifeRemaining <= 0)
            {
                this.Alpha *= .70f;
                if (Alpha < .01f)
                    MainGame.dm.GameObjects.Remove(this);
            }


            if (speed != 0) //Not yet at rest
            {
                fallVel += .1f;
                Rot += .5f * velocity.X * (rotDir ? 1 : -1);
            }


            //Pos.Y += fallVel * speed;
            velocity += new Vector2f(0, .1f);
            if (MainGame.dm.MapCollide((int)Pos.X, (int)(Pos.Y + velocity.Y / 2), CollideTypes.HardOrSoft)) //Bounce
            {
                //fallVel *= -.6f;
                //speed = 0;
                velocity.Y *= -.55f;
                velocity.X *= .6f;
                
                if (Math.Abs(velocity.Y) < .1f)
                    velocity.Y = 0;
                //else
                //    if (Math.Abs(velocity.Y) > .01f)
                //        MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("coinDrop.wav"), 1f, 0, .2f));
            }

            if (!pickedUp)
                Pos += velocity;
            if (pickedUp)
            {
                this.Pos = Helper.Lerp(this.Pos, pickerUpper.Core, pickupLerpPercent * 2);
            }

            base.Update();
        }

        public override void Draw()
        {
            if ((lifeRemaining > 120) || MainGame.rand.Next(0, 2) == 1)
            {
                Render.Draw(Texture, Pos - new Vector2f(0, 3), c, new Vector2f(Texture.Size.X / 2, Texture.Size.Y / 2), 1, Rot, 1);
            }
            base.Draw();
        }

        public void CheckCollect()
        {


            if (readyTimer < 0)
            {
                for (int i = 0; i < MainGame.dm.Players.Count; i++)
                {
                    Actor A = MainGame.dm.Players[i];
                    if (!A.Alive)
                        continue;

                    if (A.collisionBox.Contains((int)this.Pos.X, (int)this.Pos.Y - 4))
                    {
                        pickedUp = true;
                        this.angle = -Helper.angleBetween(this.Pos, A.Core);
                        pickerUpper = A;
                        pickupLerpPercent += (.15f);
                    }
                    if (Helper.Distance(this.Pos - new Vector2f(3, 0), A.Core) < 15)
                    {
                        if (A is ClientPlayer)
                        {
                            MainGame.dm.player.gold++;
                        }
                        MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("coin.wav"), 1f, .2f));
                        MainGame.dm.GameObjects.Remove(this);
                        //Play pickup sound
                    }
                }
            }

        }
    }
}
