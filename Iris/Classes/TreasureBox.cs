using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Iris
{
    public class TreasureBox : GameObject
    {
        Vector2f origin;
        bool opened = false;
        int openDelay = 60 * 2; //Timer until the chest opens
        int dropTimer = 60 * 1; //Timer until the opened chest despawns
        bool goldDropped = false;

        public TreasureBox()
        {
            
        }

        public TreasureBox(Vector2f Pos)
        {
            this.Pos = Pos;
            Alpha = 255;
            Texture = Content.GetTexture("treasureClosed.png");
            origin = new Vector2f(this.Texture.Size.X / 2, this.Texture.Size.Y);
        }

        public override void Update()
        {
            openDelay--;
            if (goldDropped)
                dropTimer--;

            if (openDelay < 0)
            {
                if (!opened)
                    Open();
            }

            if (dropTimer < 0)
            {
                Alpha *= .96f;
                if (Alpha < .01f)
                    MainGame.dm.GameObjects.Remove(this);
            }

            if (!goldDropped &&  opened)
            {
                for (int i = 0; i < MainGame.dm.Players.Count; i++)
                {
                    Actor A = MainGame.dm.Players[i];
                    if (A.collisionBox.Contains((int)this.Pos.X, (int)this.Pos.Y - 4))
                    {
                        DropMoney(200);
                        goldDropped = true;
                        Texture = Content.GetTexture("treasureEmpty.png");
                        //origin = new Vector2f(this.Texture.Size.X / 2, this.Texture.Size.Y);
                    }
                }

            }

            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(this.Texture, this.Pos + new Vector2f(0, 1), new Color(255,255,255,(byte)Alpha), origin, 1, 0f);
            base.Draw();
        }

        public void Open()
        {
            opened = true;
            MainGame.dm.GameObjects.Add(new GunSmoke(new Vector2f(Pos.X, Pos.Y - 25),0));
            this.Texture = Content.GetTexture("treasureOpen.png");
            origin = new Vector2f(47, 67);
        }

        private void DropMoney(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Coin c = new Coin(this.Pos + new Vector2f(MainGame.rand.Next(-3,4), -4), 2.5f + (float)MainGame.rand.NextDouble() * 1.5f, (float)
                    (-Math.PI / 2 + .35 * (MainGame.rand.NextDouble() - .5)));
                MainGame.dm.Mailman.SendCoinCreate(c);
                //MainGame.dm.GameObjects.Add(c);
            }
        }

    }
}
