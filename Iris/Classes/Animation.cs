using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Animation
    {
        public Texture Texture;
        public int Frame;
        public int Count;
        public int Duration;

        private float timer;
        private IntRect intRect;
        

        public Animation(Texture t, int count, int duration)
        {
            this.Count = count;
            this.Texture = t;
            this.Duration = duration * 100;
        }

        public void Update()
        {
            timer += MainGame.startTime.Millisecond;

            //Console.WriteLine(frame);
            if (timer > Duration)
            {
                Frame++;
                if (Frame >= Count)
                    Frame = 0;
                timer = 0;
            }

            int widthOfFrame = (int)(Texture.Size.X / Count);
            int heightOfFrame = (int)(Texture.Size.Y / 1);

            intRect = new IntRect(
                widthOfFrame * Frame,
                heightOfFrame * 1,
                widthOfFrame,
                heightOfFrame
            );

            //Console.WriteLine(intRect.Width);

            //sprite.TextureRect = new IntRect(0, 0, 64, 64);//intRect;
            //sprite.Origin = new Vector2f(sprite.Texture.Size.X / 2 / count, sprite.Texture.Size.Y);
        }

        public Texture getTexture()
        {
            //Update();
            return Texture;
        }

        public static void DrawAnimation(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, int totalFrames, int currentFrame, float layer = 0.0f)
        {
            int widthOfFrame = (int)(texture.Size.X / totalFrames);
            int heightOfFrame = (int)(texture.Size.Y / 1);

            IntRect source = new IntRect(
                widthOfFrame * currentFrame,
                0,
                widthOfFrame,
                heightOfFrame
            );

            DrawGenericTexture(texture, position, color, origin, facing, 0f, source, layer);
        }

        //TODO: fix facing origin (-1 doesn't reflect about its center)
        private static void DrawGenericTexture(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, float rotation, IntRect? textureRect, float layer, float scale = 1)
        {
            Sprite sprite = new Sprite(texture);
            sprite.Texture.Smooth = false;
            sprite.Scale = new Vector2f(facing, 1);
            sprite.Origin = origin;
            sprite.Position = position;
            sprite.Color = color;
            sprite.Rotation = rotation;
            sprite.Scale = new Vector2f(facing, 1) * scale;
            if (textureRect.HasValue)
            {
                sprite.TextureRect = textureRect.Value;
            }

            sprite.Draw(MainGame.window, RenderStates.Default);
        }
    }
}
