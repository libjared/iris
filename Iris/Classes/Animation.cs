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
        public Sprite sprite;
        public int frame;
        public int count;
        public int duration;

        private float timer;
        private IntRect intRect;
        

        public Animation(Sprite s, int count, int duration)
        {
            this.count = count;
            this.sprite = s;
            this.duration = duration;
        }

        public void Update()
        {
            timer += MainGame.startTime.Millisecond;

            //Console.WriteLine(frame);
            if (timer > duration)
            {
                frame++;
                if (frame >= count)
                    frame = 0;
                timer = 0;
            }

            int widthOfFrame = (int)(sprite.Texture.Size.X / count);
            int heightOfFrame = (int)(sprite.Texture.Size.Y / 1);

            intRect = new IntRect(
                widthOfFrame * frame,
                heightOfFrame * 1,
                widthOfFrame,
                heightOfFrame
            );

            //Console.WriteLine(intRect.Width);

            sprite.TextureRect = new IntRect(0, 0, 64, 64);//intRect;
            //sprite.Origin = new Vector2f(sprite.Texture.Size.X / 2 / count, sprite.Texture.Size.Y);
        }

        public Texture getTexture()
        {
            Update();
            return sprite.Texture;
        }

        public static void DrawAnimation(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, int totalFrames, int totalRows, int currentFrame, int currentRow, float layer = 0.0f)
        {
            int widthOfFrame = (int)(texture.Size.X / totalFrames);
            int heightOfFrame = (int)(texture.Size.Y / totalRows);

            IntRect source = new IntRect(
                widthOfFrame * currentFrame,
                heightOfFrame * currentRow,
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
