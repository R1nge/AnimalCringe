using _Assets.Scripts.Damageables;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class RaycastWeapon : Weapon
    {
        protected RaycastHit[] Hits;

        protected override void Awake()
        {
            base.Awake();
            Hits = new RaycastHit[5];
        }

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

        public override HitInfo Shoot(ulong owner, Vector3 origin, Vector3 direction, bool isServer)
        {
            var hitInfo = new HitInfo();

            if (CanShoot)
            {
                if (Physics.RaycastNonAlloc(origin, direction, Hits, weaponConfig.Range, ~ignoreLayer) != 0)
                {
                    //The first hit is always the player
                    for (int i = 1; i < Hits.Length; i++)
                    {
                        if (Hits[i].transform.root.TryGetComponent(out NetworkObject networkObject))
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
            }

            return hitInfo;
        }
    }
}