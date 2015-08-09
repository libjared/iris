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
        int openDelay = 60 * 8; //Timer until the chest opens
        int dropTimer = 60 * 1; //Timer until the opened chest despawns
        int inactiveTimer = 60 * 20; //Timer until an untouched chest despawns
        int goldCount = 100;
        public bool goldDropped = false;

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
            if (inactiveTimer < 0)
                MainGame.dm.GameObjects.Remove(this);

            openDelay--;
            if (goldDropped)
                dropTimer--;

            if (openDelay < 0)
            {
                if (!opened)
                    Open();
                inactiveTimer--;
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
                        MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("coinDrop.wav"), 1f, .4f, 4));
                        MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("dooropen.wav"), 1f, .4f, 10));
                        DropMoney(goldCount);
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
            if (inactiveTimer < 120 && inactiveTimer % 2 == 0)
                return;
            Render.Draw(this.Texture, this.Pos + new Vector2f(0, 1), new Color(255,255,255,(byte)Alpha), origin, 1, 0f);
            base.Draw();
        }

        public void Open()
        {
            opened = true;
            MainGame.dm.GameObjects.Add(new GunSmoke(new Vector2f(Pos.X, Pos.Y - 25),0));
            this.Texture = Content.GetTexture("treasureOpen.png");
            origin = new Vector2f(47, 67);
            MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("dooropen.wav"), 1f, .4f, 10));
        }

        private void DropMoney(int count)
        {
            MainGame.dm.Mailman.SendCoinCreate(this.Pos + new Vector2f(MainGame.rand.Next(-3, 4), -4), count);
        }

    }
}
