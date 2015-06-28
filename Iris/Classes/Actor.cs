using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class Actor
    {
        public float Speed;
        public Sprite Sprite;
        public Vector2f Velocity;
        public Vector2f Pos;
        public Animation animation;

        public int Health { get; set; }
        public string Name { get; set; }
        public long UID { get; set; }
        public bool OnGround { get; set; }
        public int JumpsLeft { get; set; }
        public int MaxJumps { get; set; }
        protected Deathmatch dm;

        public Actor(Deathmatch dm)
        {
            Health = 100;
            Name = "Cactus Fantastico";
            Pos = new Vector2f(0, 0);
            UID = 0;
            //Texture = Content.GetTexture("flint_right.png");
            //Origin = new Vector2f(Texture.Size.X /2 , Texture.Size.Y);
            this.dm = dm;
        }

        public virtual void Update()
        {
            if (animation != null) 
                animation.Update();
        }

        public virtual void Draw()
        {
           // this.TextureRect = new IntRect(0, 0, 64, 55);
            MainGame.window.Draw(this.Sprite);
        }
    }
}
