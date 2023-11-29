﻿using System;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Damageables
{
    public class Health : NetworkBehaviour, IDamageable
    {
        public event Action<float> OnHealthChanged; 
        [SerializeField] private NetworkVariable<float> health;

        public override void OnNetworkSpawn()
        {
            health.OnValueChanged += HealthChanged;
            HealthChanged(0, health.Value);
        }

        public void TakeDamage(ulong owner, int damage)
        {
            if (damage <= 0)
            {
                Debug.LogError($"Trying to take {damage} amount of damage");
                return;
            }       
            
            TakeDamageServerRpc(owner, damage);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(ulong owner, int damage)
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