using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Iris
{
    public class Revolver : Weapon
    {
        public Revolver(Actor owner) : base(owner)
        {
            this.texture = Content.GetTexture("revolver.png");
            MaxAmmo = 8;
            FireSpeed = 10;
            ReloadSpeed = 90;
            Ammo = MaxAmmo;
            AutomaticFire = false;
        }

        public override void Update()
        {
            ReloadTimer--;
            FireTimer--;

            if (ReloadTimer == 0)
            {
                Ammo = MaxAmmo;
            }

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }

        public override void Reload()
        {
            base.Reload();

            if (Ammo != 0)
            {
                ReloadTimer = ReloadSpeed;
                Ammo = 0;
            }
        }

        public override void Fire()
        {
            base.Fire();

            if (Ammo > 0)
            {
                if (FireTimer <= 0)
                {
                    MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("handgunSfx.wav"), 1f, .1f, 6));

                    //Console.WriteLine("Bang");
                    Bullet b = new Bullet(Owner.UID, Owner.AimAngle, Owner.Core + Helper.PolarToVector2(28, Owner.AimAngle, 1, 1));
                    Bullet b1 = new Bullet(Owner.UID, Owner.AimAngle - .1f, Owner.Core + Helper.PolarToVector2(28, Owner.AimAngle, 1, 1));
                    Bullet b2 = new Bullet(Owner.UID, Owner.AimAngle + .1f, Owner.Core + Helper.PolarToVector2(28, Owner.AimAngle, 1, 1));
                    Gui.CrosshairFireExpand = .75f;
                    MainGame.dm.Projectiles.Add(b);
                    MainGame.Camera.Center += Helper.PolarToVector2(3.5f * MainGame.rand.Next(1, 2), Owner.AimAngle + (float)Math.PI, 1, 1);
                    ((ClientPlayer)Owner).holdDistance = -10f;
                    MainGame.dm.GameObjects.Add(new GunSmoke(Owner.Core + Helper.PolarToVector2(32, Owner.AimAngle, 1, 1) + (Owner.Velocity), Owner.AimAngle));
                    MainGame.dm.GameObjects.Add(new GunFlash(Owner.Core + Helper.PolarToVector2(32, Owner.AimAngle, 1, 1) + (Owner.Velocity), Owner.AimAngle));
                    Ammo--;

                    if (Ammo == 0)
                        if (ReloadTimer < 0)
                            ReloadTimer = ReloadSpeed;

                    FireTimer = FireSpeed;
                    MainGame.dm.Mailman.SendBulletCreate(b);
                }
            }
            else
            {
                MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("dryFireSfx.wav"), 1f, .1f, 1));
                if (ReloadTimer < 0)
                    ReloadTimer = 70;
            }
        }


    }
}
