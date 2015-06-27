using SFML.Window;
using SFML.System;
using SFML.Graphics;

namespace Iris
{
    class ClientPlayer : Player
    {
        public ClientPlayer(Deathmatch dm)
            : base(dm)
        {
        }

        public override void Update()
        {
            Vector2f pos = Position;
            if (Keyboard.IsKeyPressed(Keyboard.Key.W)) { pos.Y--; }
            if (Keyboard.IsKeyPressed(Keyboard.Key.S)) { pos.Y++; }
            if (Keyboard.IsKeyPressed(Keyboard.Key.A)) { pos.X--; }
            if (Keyboard.IsKeyPressed(Keyboard.Key.D)) { pos.X++; }
            Position = pos;

            if (dm.MapCollide((int)Position.X, (int)Position.Y))
            {
                Color = Color.Green;
            }
            else
            {
                Color = Color.White;
            }

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
