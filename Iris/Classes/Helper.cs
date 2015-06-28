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
        public static Vector2f PolarToVector2(float radius, float angle, float xPercent, float yPercent)
        {
            return new Vector2f(radius * (float)Math.Cos((double)angle) * xPercent, radius * (float)Math.Sin((double)angle) * yPercent);
        }

        public static float Distance(Vector2f vec1, Vector2f vec2)
        {
            return (float)Math.Sqrt(Math.Pow((vec2.X - vec1.X), 2) + Math.Pow((vec2.Y - vec1.Y), 2));
        }

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

        public static float DegToRad(float degree)
        {
            return degree * ((float)Math.PI / 180f);
        }

        public static float RadToDeg(float rad)
        {
            return rad * (180f / (float)Math.PI);
        }
    }
}
