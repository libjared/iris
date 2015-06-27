using SFML.Window;
using SFML.System;

namespace Iris
{
    class ClientPlayer : Player
    {
        public ClientPlayer()
            : base()
        {

        }

        public override void Update()
        {
            if (Input.isKeyDown(Keyboard.Key.S))
            {
                Position = new Vector2f(20, 20);
            }
            else
            {
                Position = new Vector2f(0, 0);
            }

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
