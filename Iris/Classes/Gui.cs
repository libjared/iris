using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public static class Gui
    {
        //for things only gui should hold
        public static float CrosshairFireExpand = 0f;

        //for things normally held in other classes; updated framely
        //private static 

        public static void Update()
        {
        }

        public static void Draw()
        {
            Vector2f mouse = MainGame.window.MapPixelToCoords(Input.screenMousePos);

            Vector2u crosshairOriginU = Content.GetTexture("crosshair.png").Size / 2;
            Vector2f crosshairOrigin = new Vector2f(crosshairOriginU.X, crosshairOriginU.Y);
            Render.Draw(Content.GetTexture("healthBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("ammoBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("crosshair.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
        }
    }
}
