﻿using System;
using System.Collections;
using _Assets.Scripts.Players;
using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Damageables
{
    public class Health : NetworkBehaviour, IDamageable
    {
        public event Action<float> OnHealthChanged;
        [SerializeField] private float maxHealth;
        [SerializeField] private float invincibilityTime;
        private NetworkVariable<float> _health;
        private NetworkVariable<bool> _invincible;
        private KillService _killService;
        private PlayerDeathController _playerDeathController;

        [Inject]
        private void Inject(KillService killService) => _killService = killService;

        private void Awake()
        {
            _playerDeathController = GetComponent<PlayerDeathController>();
            _health = new NetworkVariable<float>(maxHealth);
            _invincible = new NetworkVariable<bool>(true);
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

            if (_health.Value - damage <= 0)
            {
                DieServerRpc(killer);
            }
            else
            {
                TakeDamageServerRpc(killer, damage);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void DieServerRpc(ulong killerId)
        {
            _killService.KillServerRpc(OwnerClientId, killerId);
            _playerDeathController.Die();
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(ulong killer, int damage)
        {
            _health.Value -= damage;
            Debug.LogError($"Current health of {OwnerClientId} is {_health.Value}");
        }

        private void HealthChanged(float _, float value) => OnHealthChanged?.Invoke(value);

        public override void OnDestroy()
        {
            base.OnDestroy();
            _health.OnValueChanged -= HealthChanged;
        }
    }
}