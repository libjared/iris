﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class CliffFace : GameObject
    {
        int yKill = 430;
        int xKill = 320;
        bool wooshed = false;

        public CliffFace()
        {
            this.Texture = Content.GetTexture("CliffOpenBlack.png");
            this.Pos.X = 3500;
        }

        public override void Update()
        {
            if (MainGame.dm.player.Pos.X < this.Pos.X + xKill * 2)
                MainGame.dm.tunnel = true;

            this.Pos.Y = -345;
            this.Pos.X -= 15;

            if (this.Pos.X < MainGame.dm.player.Pos.X && !wooshed)
            {
                MainGame.Camera.Center -= new Vector2f(2000f / (float)(MainGame.Camera.Center.X - (this.Pos.X + xKill)), 0);
                wooshed = true;
            }


            Actor a = MainGame.dm.player;
            if (a.Pos.X > this.Pos.X + xKill &&
                a.Pos.Y < this.Pos.Y + yKill &&
                a.Pos.X < this.Pos.X + this.Texture.Size.X * 2 - xKill)
            {
                a.Health = 0;
            }

            //search for dynamite and fling them left
            MainGame.dm.Projectiles
                .Where((o) => { return o is BombInstance; })
                .Cast<BombInstance>()
                .Where((o) => {
                    return o.Pos.X > Pos.X + xKill &&
                    o.Pos.Y < Pos.Y + yKill &&
                    o.Pos.X < Pos.X + Texture.Size.X * 2 - xKill; })
                .ToList()
                .ForEach((o) =>
                {
                    o.Velocity = new Vector2f(-2f, 2f);
                });

            base.Update();
    }

    public override void Draw()
    {
        Render.Draw(this.Texture, this.Pos, Color.White, new Vector2f(0, 0), 1, 0f, 1);
        Render.Draw(this.Texture, this.Pos + new Vector2f(this.Texture.Size.X * 2, 0), Color.White, new Vector2f(0, 0), 1, 0f, 1, false, true);
        base.Draw();
    }
}
}
