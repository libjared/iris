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
            
            Render.Draw(Content.GetTexture("healthBarVert.png"), mouse, Color.White, new Vector2f(11, 11), 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("ammoBarVert.png"), mouse, Color.White, new Vector2f(11, 11), 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("crosshair.png"), mouse, Color.White, new Vector2f(11, 11), 1, 0, 1 + CrosshairFireExpand);
        }
    }
}
