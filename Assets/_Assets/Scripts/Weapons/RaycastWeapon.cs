using _Assets.Scripts.Damageables;
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

        public override void Shoot(ulong owner, Vector3 origin, Vector3 direction)
        {
            if (CanShoot.Value)
            {
                Debug.LogError("Shot");
                animator.Animator.SetTrigger("Shooting");
                if (Physics.Raycast(origin, direction, out RaycastHit hit))
                {
                    if (hit.transform.TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(owner, weaponConfig.Damage);
                    }
                }
            }
        }
    }
}