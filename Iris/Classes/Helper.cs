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
            return new Vector2f(radius * (float)Math.Cos(angle) * xPercent,
                                radius * (float)Math.Sin(angle) * yPercent);
        }

        public static Vector2f Lerp(Vector2f value1, Vector2f value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
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

        public static void MoveCameraTo(View camera, Vector2f focus, float speed)
        {
            if (camera.Center.X > focus.X)
            {
                camera.Center -= new Vector2f((Math.Abs(camera.Center.X - focus.X) * speed), 0);
            }
            if (camera.Center.X < focus.X)
            {
                camera.Center += new Vector2f((Math.Abs(camera.Center.X - focus.X) * speed),0);
            }
            if (camera.Center.Y > focus.Y)
            {
                camera.Center -= new Vector2f(0,(Math.Abs(camera.Center.Y - focus.Y) * speed));
            }
            if (camera.Center.Y < focus.Y)
            {
                camera.Center += new Vector2f(0,(Math.Abs(camera.Center.Y - focus.Y) * speed));
            }
        }

        public static float Clamp(float min, float val, float max)
        {
            if (val <= min) { return min; }
            if (val >= max) { return max; }
            return val;
        }

        public static float ClampSigned(float minmax, float val)
        {
            if (Math.Abs(val) >= minmax)
            {
                return Math.Sign(val) * minmax;
            }
            return val;
        }
    }
}
