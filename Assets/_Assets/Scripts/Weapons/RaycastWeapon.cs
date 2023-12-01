﻿using _Assets.Scripts.Damageables;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class RaycastWeapon : Weapon
    {
        protected override void OnTick()
        {
            Debug.LogError($"TICK. TimeBeforeNextShot: {TimeBeforeNextShot}");
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

        public override bool Shoot(ulong owner, Vector3 origin, Vector3 direction, bool isServer)
        {
            Debug.LogError($"Server?: {isServer}, Can shoot?: {CanShoot}");
            if (CanShoot)
            {
                //animator.SetTrigger("Shooting");
                if (Physics.Raycast(origin, direction, out RaycastHit hit))
                {
                    Debug.LogError($"RAYCAST IsServer {isServer}");
                    if (hit.transform.root.TryGetComponent(out NetworkObject networkObject))
                    {
                        if (networkObject.OwnerClientId == owner)
                        {
                            Debug.LogError("Hit himself");
                            return false;
                        }

                        if (networkObject.TryGetComponent(out IDamageable damageable))
                        {
                            Debug.LogError("HIT");
                            if (isServer)
                            {
                                Debug.LogError("DAMAGE");
                                damageable.TakeDamage(owner, weaponConfig.Damage);
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}