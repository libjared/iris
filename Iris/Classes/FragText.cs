using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class FragText
    {

        public string killer, victim;
        public Texture icon;
        
        public FragText(string killer, string victim, Texture icon)
        {
            this.killer = killer;
            this.icon = icon;
            this.victim = victim;
        }
    }
}
