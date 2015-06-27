using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class Player : Sprite
    {
        public int Health { get; set; }
        public string Name { get; set; }
        public long UID { get; set; }

        public Player()
        {
            Health = 100;
            Name = "Cactus Fantastico";
            Position = new Vector2f(0, 0);
            UID = 0;
            Texture = Content.GetTexture("flint_right.png");
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
            MainGame.window.Draw(this);
        }
    }
}
