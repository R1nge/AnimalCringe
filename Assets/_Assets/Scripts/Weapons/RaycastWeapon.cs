using _Assets.Scripts.Damageables;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class RaycastWeapon : Weapon
    {
        protected readonly RaycastHit[] Hits = new RaycastHit[5];

        public override void OnTick()
        {
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

        public override HitInfo Shoot(ulong owner, Vector3 origin, Vector3 direction)
        {
            var hitInfo = new HitInfo();

            if (CanShoot)
            {
                var hits = Physics.RaycastNonAlloc(origin, direction, Hits, weaponConfig.Range); 
                if (hits != 0)
                {
                    for (int i = 0; i < hits; i++)
                    {
                        Debug.LogError($"Hits: {Hits[i].transform.name}");
                        if (Hits[i].transform.TryGetComponent(out NetworkObject networkObject))
                        {
                            hitInfo.Hit = true;

                            if (networkObject.OwnerClientId == owner)
                            {
                                continue;
                            }

                            if (networkObject.TryGetComponent(out IDamageable damageable))
                            {
                                hitInfo.VictimId = networkObject.OwnerClientId;

                                if (networkObject.OwnerClientId != owner)
                                {
                                    damageable.TakeDamage(owner, weaponConfig.Damage);
                                }

                                return hitInfo;
                            }
                        }
                    }
                }
            }

            hitInfo.VictimId = ulong.MaxValue;

            return hitInfo;
        }
    }
}