using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace Iris
{
    class NetPlayer : Player
    {
        public NetPlayer(Deathmatch dm, long UID)
            : base(dm)
        {
            this.UID = UID;
            Color = Color.Red;
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
