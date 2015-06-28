﻿using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public static class Helper
    {
        public static float angleBetween(Vector2f vec1, Vector2f vec2)
        {
            return (float)Math.PI / 2 - (float)Math.Atan2(vec1.X - vec2.X, vec1.Y - vec2.Y);
        }

        public static Vector2f normalize(Vector2f vector)
        {
            float L = length(vector);
 
            if(L != 0){
                vector.X = vector.X/L;
                vector.Y = vector.Y/L;
            }
            return vector;
        }

        public static float length(Vector2f vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
    }
}
