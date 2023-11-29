using System;
using System.Collections;
using _Assets.Scripts.Services;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Damageables
{
    public class Health : NetworkBehaviour, IDamageable
    {
        public event Action<float> OnHealthChanged;
        [SerializeField] private NetworkVariable<float> health;
        [SerializeField] private float invincibilityTime;
        private NetworkVariable<bool> _invincible;
        private KillService _killService;

        [Inject]
        private void Inject(KillService killService) => _killService = killService;

        private void Awake() => _invincible = new NetworkVariable<bool>(true);

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                health.OnValueChanged += HealthChanged;
                HealthChanged(0, health.Value);
                MakeInvincibleServerRpc();
            }
        }

        [ServerRpc]
        private void MakeInvincibleServerRpc() => StartCoroutine(MakeInvincble_C());

        private IEnumerator MakeInvincble_C()
        {
            _invincible.Value = true;
            yield return new WaitForSeconds(invincibilityTime);
            _invincible.Value = false;
        }

        public void TakeDamage(ulong killer, int damage)
        {
            if (_invincible.Value)
            {
                Debug.LogError("Trying to take damage while invincible");
                return;
            }

            if (damage <= 0)
            {
                Debug.LogError($"Trying to take {damage} amount of damage");
                return;
            }

            if (health.Value - damage <= 0)
            {
                DieServerRpc(killer);
            }
            else
            {
                TakeDamageServerRpc(killer, damage);
            }
        }

        //TODO: queue for a respawn
        [ServerRpc(RequireOwnership = false)]
        private void DieServerRpc(ulong killerId)
        {
            _killService.KillServerRpc(OwnerClientId, killerId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(ulong killer, int damage)
        {
            health.Value -= damage;
            Debug.LogError($"Current health of {OwnerClientId} is {health.Value}");
        }

        private void HealthChanged(float _, float value) => OnHealthChanged?.Invoke(value);

        public override void OnDestroy()
        {
            base.OnDestroy();
            health.OnValueChanged -= HealthChanged;
        }
    }
}