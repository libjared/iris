using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class EmoteBubble : GameObject
    {
        //1 Laugh
        //2 Cool
        //3 Shocked
        //4 Silly

        public string emote;
        public Actor parent;
        public Texture texture;
        public int life;

        public EmoteBubble(string emote, Actor parent)
        {
            this.emote = emote.ToLower();
            this.parent = parent;
            this.texture = Content.GetTexture("emote_" + emote +".png");
            life = 60 * 4;
        }

        public override void Update()
        {
            if (life <= 0)
                MainGame.dm.GameObjects.Remove(this);
            this.Pos = parent.Pos - new Vector2f(35, 85);
            life--;
            base.Update();
        }

        public override void Draw()
        {
            Render.Draw(this.texture, this.Pos, Color.White, new Vector2f(0, 0), 1, 0f);
            base.Draw();
        }

    }
}
