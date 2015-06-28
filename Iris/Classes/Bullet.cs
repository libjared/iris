using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Bullet : Projectile
    {
        public Bullet(float angle, Vector2f position, float speed, int damage)
        {
            this.Angle = angle;
            this.Pos = position;
            this.Speed = speed;
            this.Damage = damage;
        }

        public override void Update()
        { 
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
