using _Assets.Scripts.Damageables;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class RaycastWeapon : Weapon
    {
        public override void OnTick(bool isServer)
        {
            Debug.LogError("WEAPON TICK");
            if (!CanShoot)
            {
                if (TimeBeforeNextShot <= 0)
                {
                    CanShoot = true;
                    TimeBeforeNextShot = 1 / (weaponConfig.FireRate / 60f);
                }
                else
                {
                    CanShoot = false;
                    TimeBeforeNextShot -= 1f / NetworkManager.Singleton.NetworkTickSystem.TickRate;
                }
            }
        }

        public override HitInfo Shoot(ulong owner, Vector3 origin, Vector3 direction, bool isServer)
        {
            var hitInfo = new HitInfo();

            //So, the ray goes through the clients collider, thats why the client can't shoot host

            if (CanShoot)
            {
                if (Physics.Raycast(origin, direction, out RaycastHit hit, weaponConfig.Range, ~ignoreLayer))
                {
                    if (hit.transform.root.TryGetComponent(out NetworkObject networkObject))
                    {
                        hitInfo.Hit = true;

                        if (networkObject.OwnerClientId == owner)
                        {
                            return hitInfo;
                        }

                        if (networkObject.TryGetComponent(out IDamageable damageable))
                        {
                            hitInfo.VictimId = networkObject.OwnerClientId;

                            if (isServer)
                            {
                                damageable.TakeDamage(owner, weaponConfig.Damage);
                            }

                            return hitInfo;
                        }
                    }
                }
            }

            return hitInfo;
        }
    }
}