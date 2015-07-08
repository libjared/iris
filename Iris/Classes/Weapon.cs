using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class Weapon
    {
        public int fireSpeed; //Duration between each fire
        public int ammo; //Current ammo left
        public int maxAmmo; //Default amount of ammo
        public int reloadSpeed; //How long it takes to reload the weapon entirely

        public Weapon()
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
