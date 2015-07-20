using SFML.Window;
using SFML.System;
using SFML.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Iris
{
    public class BombWeapon : Weapon
    {
        public BombWeapon(Actor owner)
            : base(owner)
        {
            this.texture = Content.GetTexture("bombWeapon.png");
            MaxAmmo = 2;
            FireSpeed = 30;
            ReloadSpeed = 1;
            Ammo = MaxAmmo;
            AutomaticFire = false;
        }

        public override void Update()
        {
            ReloadSpeed = (MaxAmmo - Ammo) * 10;
            ReloadTimer--;
            FireTimer--;

            if (FireTimer > 0)
                this.texture = null;
            else
                this.texture = Content.GetTexture("bombWeapon.png");

            if (ReloadTimer == 0)
            {
                Ammo = MaxAmmo;
            }

            if (Input.isMouseButtonTap(Mouse.Button.Right))
            {
                MainGame.dm.GameObjects.Add(new Explosion(this.Owner.Core));
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
                    //Console.WriteLine("Bang");
                    
                    BombInstance b = new BombInstance(Owner.UID, Owner.AimAngle, Owner.Core + Helper.PolarToVector2(28, Owner.AimAngle, 1, 1));
                    MainGame.dm.Mailman.SendBombCreate(b);
                    Gui.CrosshairFireExpand = .75f;
                    MainGame.dm.Projectiles.Add(b);
                    MainGame.Camera.Center += Helper.PolarToVector2(3.5f * MainGame.rand.Next(2, 3), Owner.AimAngle + (float)Math.PI, 1, 1);
                    ((ClientPlayer)Owner).holdDistance = -10f;
                    //MainGame.dm.GameObjects.Add(new GunSmoke(Owner.Core + Helper.PolarToVector2(32, Owner.AimAngle, 1, 1) + (Owner.Velocity), Owner.AimAngle));
                    //MainGame.dm.GameObjects.Add(new GunFlash(Owner.Core + Helper.PolarToVector2(32, Owner.AimAngle, 1, 1) + (Owner.Velocity), Owner.AimAngle));
                    Ammo--;

                    if (Ammo == 0)
                        if (ReloadTimer < 0)
                            ReloadTimer = ReloadSpeed;

                    FireTimer = FireSpeed;
                    //MainGame.dm.Mailman.SendBulletCreate(b);
                }
            }
            else
            {
                if (ReloadTimer < 0)
                    ReloadTimer = 70;
            }
        }


    }
}
