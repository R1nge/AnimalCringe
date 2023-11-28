using System;
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
        private DamagePopupService _damagePopupService;
        
        
        [Inject]
        private void Inject(DamagePopupService damagePopupService)
        {
            _damagePopupService = damagePopupService;
        }

        public override void OnNetworkSpawn()
        {
            health.OnValueChanged += HealthChanged;
            HealthChanged(0, health.Value);
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                Debug.LogError($"Trying to take {damage} amount of damage");
                return;
            }       
            
            TakeDamageServerRpc(damage);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(int damage, ServerRpcParams rpcParams = default)
        {
            health.Value -= damage;

            var clientRpc = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new []
                    {
                        rpcParams.Receive.SenderClientId
                    }
                }
            };
            
            _damagePopupService.ShowPopupClientRpc(transform.position, 1f, clientRpc);
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