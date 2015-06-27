using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class Player
    {
        public int Health { get; set; }
        public string Name { get; set; }
        public Vector2i Position { get; set; }
        public long UID { get; set; }
    }
}
