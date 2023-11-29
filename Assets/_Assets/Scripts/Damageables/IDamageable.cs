using UnityEngine;

namespace _Assets.Scripts.Damageables
{
    public interface IDamageable
    {
        void TakeDamage(ulong owner, int damage, Vector3 hitPosition, Vector3 hitDirection);
    }
}