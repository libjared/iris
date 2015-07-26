using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Menu : Gamestate
    {
        private static RenderStates shader;

        public Menu()
            : base()
        {
            shader = new RenderStates(new Shader(null, "Content/bgPrlx.frag"));
        }


        public override void Update()
        {
            //Main menu logic
            //Probably just a username and an IP to connect to
        }

        public override void Draw()
        {
            shader.Shader.SetParameter("offsetY", 0);
            RectangleShape rs = new RectangleShape
            {
                Size = new Vector2f(800, 600)
            };
            MainGame.window.Draw(rs, shader);
            //MainGame.window.SetView(MainGame.Camera);

            Render.Draw(Content.GetTexture("background1Far.png"), new Vector2f(-200, -100), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture("background1Far.png"), new Vector2f(145, -100), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture("background1.png"), new Vector2f(-200,-150), Color.White, new Vector2f(0,0), 1, 0f);

            Render.Draw(Content.GetTexture("cursorPointer.png"), (Vector2f)MainGame.worldMousePos, Color.White, new Vector2f(0, 0), 1, 0f);
        }
    }
}
