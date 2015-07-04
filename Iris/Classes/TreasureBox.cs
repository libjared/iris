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
        public TreasureBox()
        {
            
        }

        public TreasureBox(Vector2f Pos)
        {
            this.Pos = Pos;
            Texture = Content.GetTexture("treasureClosed.png");
        }

        public override void Update()
        {
            origin = new Vector2f(this.Texture.Size.X / 2, this.Texture.Size.Y);
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(this.Texture, this.Pos + new Vector2f(0,1), Color.White, origin, 1, 0f);
            base.Draw();
        }

        public void Destroy()
        {
            DropMoney(200);
        }

        private void DropMoney(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Coin c = new Coin(this.Pos - new Vector2f(0, 0), 2.5f + (float)MainGame.rand.NextDouble() * 1.5f, (float)
                    (-Math.PI / 2 + .35 * (MainGame.rand.NextDouble() - .5)));
                //dm.Mailman.SendCoinCreate(c);
                MainGame.dm.GameObjects.Add(c);
            }
        }

    }
}
