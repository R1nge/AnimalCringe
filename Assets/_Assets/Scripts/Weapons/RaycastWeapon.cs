using _Assets.Scripts.Damageables;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class RaycastWeapon : Weapon
    {
        protected override void OnTick()
        {
            if (!IsServer) return;
            if (!IsSpawned) return;

            if (TimeBeforeNextShot <= 0)
            {
                CanShoot.Value = true;
                TimeBeforeNextShot = 1 / (weaponConfig.FireRate / 60f);
            }
            else
            {
                CanShoot.Value = false;
                TimeBeforeNextShot -= 1f / NetworkManager.NetworkTickSystem.TickRate;
            }
        }

        public override bool Shoot(ulong owner, Vector3 origin, Vector3 direction, int tick)
        {
            if (CanShoot.Value)
            {
                animator.Animator.SetTrigger("Shooting");
                if (Physics.Raycast(origin, direction, out RaycastHit hit))
                {
                    if (hit.transform.root.TryGetComponent(out NetworkObject networkObject))
                    {
                        if (networkObject.OwnerClientId == owner)
                        {
                            Debug.LogError("Hit himself");
                            return false;
                        }
                    }

                    if (hit.transform.root.TryGetComponent(out IDamageable damageable))
                    {
                        if (IsServer)
                        {
                            damageable.TakeDamage(owner, weaponConfig.Damage);
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}