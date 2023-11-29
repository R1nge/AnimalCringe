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

        public void TakeDamage(ulong owner, int damage, Vector3 hitPosition, Vector3 hitDirection)
        {
            if (damage <= 0)
            {
                Debug.LogError($"Trying to take {damage} amount of damage");
                return;
            }       
            
            TakeDamageServerRpc(owner, damage, hitPosition, hitDirection);
        }

        [ServerRpc(RequireOwnership = false)]
        private void TakeDamageServerRpc(ulong owner, int damage, Vector3 hitPosition, Vector3 hitDirection)
        {
            health.Value -= damage;

            var clientRpc = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new []
                    {
                        owner
                    }
                }
            };
            
            _damagePopupService.ShowPopupClientRpc(hitPosition, hitDirection, 1f, damage, clientRpc);
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