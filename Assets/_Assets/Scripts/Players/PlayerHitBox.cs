using _Assets.Scripts.Damageables;
using UnityEngine;

namespace _Assets.Scripts.Players
{
    public class PlayerHitBox : MonoBehaviour, IDamageable
    {
        private Health _health;

        private void Start() => _health = transform.root.GetComponent<Health>();

        public void TakeDamage(ulong killer, int damage)
        {
            _health.TakeDamage(killer, damage);
        }
    }
}