﻿using SFML.Window;
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
        public float Rot;
        public Vector2f Pos;
        public Vector2f Velocity;
        public float Speed;
        public float Damage;

        public Projectile()
        {
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            
        }
    }
}
