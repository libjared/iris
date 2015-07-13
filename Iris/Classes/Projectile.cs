using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Iris
{
    public abstract class Projectile : GameObject
    {
        public float Angle;
        public Actor Owner;
        public Vector2f Velocity;
        public float Speed;
        public int Damage;
        public float Direction;
        public float weightFactor;
        public long OwnerUID;

        public Projectile()
        {
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            
        }

        public virtual void onPlayerHit(Actor hit)
        {
            //If we ever use a hit.OnHit() method
        }

        public virtual void Destroy()
        {

        }
    }
}
