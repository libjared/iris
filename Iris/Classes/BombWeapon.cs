using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public class BombWeapon : Weapon
    {
        const int BombThrowRadius = 28;
        const float CrosshairFireExpand = 0.75f;
        const float CameraRecoil = 3.5f;
        const int CameraRecoilRandMin = 2;
        const int CameraRecoilRandMax = 3;
        const float ArmRecoil = -10f;

        public BombWeapon(Actor owner)
            : base(owner)
        {
            texture = Content.GetTexture("bombWeapon.png");
            MaxAmmo = 2;
            FireSpeed = 30;
            ReloadSpeed = 1;
            Ammo = MaxAmmo;
            AutomaticFire = false;
        }

        public override void Update()
        {
            base.Update();

            ReloadSpeed = (MaxAmmo - Ammo) * 10;
            ReloadTimer--;
            FireTimer--;

            //no bomb in hand when it was just thrown
            if (FireTimer > 0)
                texture = null;
            else
                texture = Content.GetTexture("bombWeapon.png");

            if (ReloadTimer == 0)
            {
                Ammo = MaxAmmo;
            }

            if (Input.isMouseButtonTap(Mouse.Button.Right))
            {
                MainGame.dm.GameObjects.Add(new Explosion(this.Owner.Core));
            }
        }

        public override void Reload()
        {
            base.Reload();

            //mid-clip reload
            if (Ammo != 0)
            {
                ReloadTimer = ReloadSpeed;
                Ammo = 0;
            }
        }

        public override void Fire()
        {
            base.Fire();

            //if can fire
            if (FireTimer <= 0)
            {
                BombInstance b = new BombInstance(Owner.UID, Owner.AimAngle, Owner.Core + Helper.PolarToVector2(BombThrowRadius, Owner.AimAngle, 1, 1));
                MainGame.dm.Projectiles.Add(b);
                MainGame.dm.Mailman.SendBombCreate(b);
                Gui.CrosshairFireExpand = CrosshairFireExpand;

                //two kinds of recoil
                MainGame.Camera.Center +=
                    Helper.PolarToVector2(CameraRecoil * MainGame.rand.Next(CameraRecoilRandMin, CameraRecoilRandMax),
                                          Owner.AimAngle + (float)Math.PI, 1, 1);
                ((ClientPlayer)Owner).holdDistance = ArmRecoil;

                Ammo--;

                //auto reload
                if (Ammo == 0)
                    if (ReloadTimer < 0)
                        ReloadTimer = ReloadSpeed;

                FireTimer = FireSpeed;
            }
        }
    }
}
