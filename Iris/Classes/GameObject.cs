﻿using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class GameObject
    {
        public Texture Texture;

        public GameObject()
        {
        }

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {

        }
    }
}
