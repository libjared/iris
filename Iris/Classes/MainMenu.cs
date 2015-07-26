using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class MainMenu
    {
        public void Update()
        {
            //Main menu logic
            //Probably just a username and an IP to connect to
        }

        public void Draw()
        {
            Render.Draw(Content.GetTexture("background1.pmg"), new Vector2f(0,0), Color.White, new Vector2f(0,0), 1, 0f);
        }
    }
}
