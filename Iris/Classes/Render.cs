using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public static class Render
    {
        public static RenderStates? renderStates = null;

        public static void Draw(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, float rotation, float scale = 1f, bool flipVert = false, bool flipHoriz = false)
        {
            DrawGenericTexture(texture, position, color, origin, facing, rotation, null, scale, flipVert, flipHoriz);
        }

        public static void DrawCircle(Vector2f pos, float radius)
        {
            var a = new CircleShape(radius)
            {
                Position = pos,
                FillColor = Color.White,
                Radius = radius,
                Origin = new Vector2f(radius, radius),
            };
            
            if (renderStates.HasValue)
            {
                MainGame.window.Draw(a, renderStates.Value);
            }
            else
            {
                MainGame.window.Draw(a);
            }
        }

        public static void DrawString(Font font, String message, Vector2f position, Color color, float scale, bool centered, float layer = 0.0f, bool rightJust = false)
        {
            Text text = new Text(message, font);
            text.Scale = new Vector2f(scale, scale);
            text.Position = position;
            text.Color = color;
            if (centered)
                text.Position = new Vector2f(text.Position.X - ((text.GetLocalBounds().Width * scale) / 2), text.Position.Y);
            if (rightJust)
                text.Position = new Vector2f(text.Position.X - ((text.GetLocalBounds().Width * scale)), text.Position.Y);

            MainGame.window.Draw(text);
        }

        public static void DrawAnimation(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, int totalFrames, int currentFrame, float layer = 0.0f, float angle = 0f, float scale = 1)
        {
            int widthOfFrame = (int)(texture.Size.X / totalFrames);
            int heightOfFrame = (int)(texture.Size.Y / 1);

            IntRect source = new IntRect(
                widthOfFrame * currentFrame,
                0,
                widthOfFrame,
                heightOfFrame
            );

            DrawGenericTexture(texture, position, color, origin, facing, angle, source, scale);
        }

        //TODO: fix facing origin (-1 doesn't reflect about its center)
        private static void DrawGenericTexture(Texture texture, Vector2f position, Color color, Vector2f origin, int facing, float rotation, IntRect? textureRect, float scale = 1, bool flipVert = false, bool flipHoriz = false)
        {
            Sprite sprite = new Sprite(texture);
            sprite.Texture.Smooth = false;
            sprite.Scale = new Vector2f(facing, 1);
            sprite.Origin = origin;
            sprite.Position = position;
            sprite.Color = color;
            sprite.Rotation = Helper.RadToDeg(rotation);
            sprite.Scale = new Vector2f(facing * (flipHoriz? -1 : 1), flipVert? -1 : 1) * scale;

            if (textureRect.HasValue)
            {
                sprite.TextureRect = textureRect.Value;
            }

            if (renderStates.HasValue)
            {
                MainGame.window.Draw(sprite, renderStates.Value);
            } else {
                MainGame.window.Draw(sprite);
            }
        }

        public static void OffsetPosition(Vector2f offset)
        {
            //offsetPosition = offset;
        }
    }
}
