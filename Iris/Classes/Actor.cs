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
        public Actor Killer; // The person that kills them
        public bool Alive;
        public int AliveTimer;
        public Texture Texture;
        public Vector2f Velocity;
        public Vector2f Pos;
        public Animation animation;
        public int Facing = 1;
        public float AimAngle;
        public Vector2f Core;
        public int ouchTimer;
        public IntRect collisionBox;
        public bool initialized;
        public string Name = "Actor";
        public Model model;
        public int gold = 100;


        public int Health { get; set; }
        public long UID { get; set; }
        public bool OnGround { get; set; }
        public int JumpsLeft { get; set; }
        public int MaxJumps { get; set; }
        protected Deathmatch dm;
        protected static RenderStates shader;

        public Weapon weapon;

        public List<Weapon> weapons = new List<Weapon>() { };

        static Actor()
        {
            var sh = new Shader(null, "Content/setWhite.frag");
            shader = new RenderStates(sh);
        }

        public Actor(Deathmatch dm)
        {
            Health = 100;
            Pos = new Vector2f(0, 0);
            UID = 0;
            Texture = null;//new Texture();
            //Texture = Content.GetTexture("flint_right.png");
            //Origin = new Vector2f(Texture.Size.X /2 , Texture.Size.Y);
            this.dm = dm;
            Killer = this;
        }

        public virtual void Update()
        {
            if (Health > 0)
                Alive = true;
            else
                Alive = false;

            if (Alive)
                AliveTimer++;
            else
                AliveTimer = 0;

            if (animation != null) 
                animation.Update();
            ouchTimer--;
        }

        public virtual void Draw()
        {
           // this.TextureRect = new IntRect(0, 0, 64, 55);
            //MainGame.window.Draw(this.Sprite);
        }

        public virtual void OnProjectileHit(Projectile hit)
        {
        }

        public virtual void UpdateCollisionBox()
        {

        }
    }
}
