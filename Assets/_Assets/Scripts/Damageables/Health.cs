using System;
using System.Collections;
using _Assets.Scripts.Players;
using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Damageables
{
    public class Health : NetworkBehaviour
    {
        public event Action<float> OnHealthChanged;
        [SerializeField] private float maxHealth;
        [SerializeField] private float invincibilityTime;
        private NetworkVariable<float> _health;
        private NetworkVariable<bool> _invincible;
        private NetworkVariable<bool> _isDead;
        private KillService _killService;
        private PlayerDeathController _playerDeathController;

        [Inject]
        private void Inject(KillService killService) => _killService = killService;

        private void Awake()
        {
            _playerDeathController = GetComponent<PlayerDeathController>();
            _health = new NetworkVariable<float>(maxHealth);
            _invincible = new NetworkVariable<bool>(true);
            _isDead = new NetworkVariable<bool>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _health.OnValueChanged += HealthChanged;
                HealthChanged(0, _health.Value);
                MakeInvincibleServerRpc();
            }
        }

        public void Respawn()
        {
            MakeInvincibleServerRpc();
            _health.Value = maxHealth;
        }

        [ServerRpc(RequireOwnership = false)]
        private void MakeInvincibleServerRpc() => StartCoroutine(MakeInvincble_C());

        private IEnumerator MakeInvincble_C()
        {
            _isDead.Value = false;
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

            if (_isDead.Value)
            {
                Debug.LogError("Trying to take damage while dead");
                return;
            }

            if (damage <= 0)
            {
                Debug.LogError($"Trying to take {damage} amount of damage");
                return;
            }

            if (_health.Value - damage <= 0)
            {
                Die(killer);
            }
            else
            {
                _health.Value -= damage;
            }
        }

        private void Die(ulong killerId)
        {
            _isDead.Value = true;
            _killService.KillServerRpc(OwnerClientId, killerId);
            _playerDeathController.Die();
        }

        private void HealthChanged(float _, float value) => OnHealthChanged?.Invoke(value);

        public override void OnDestroy()
        {
            base.OnDestroy();
            _health.OnValueChanged -= HealthChanged;
        }
    }
}