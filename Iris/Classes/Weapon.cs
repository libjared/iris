using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public abstract class Weapon
    {
        public int FireSpeed; //Duration between each fire
        public int FireTimer; //Timer between each fire
        public int Ammo; //Current ammo left
        public int MaxAmmo; //Default amount of ammo
        public int ReloadSpeed; //How long it takes to reload the weapon entirely
        public int ReloadTimer; //Timer of reloading
        public Actor Owner; //The actor holding onto the weapon
        public Texture texture;
        public bool AutomaticFire;

        public Weapon(Actor owner)
        {
            Owner = owner;
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void Fire()
        {
        }

        public virtual void Reload()
        {
        }
    }
}
