using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Players
{
    public class PlayerRollback : NetworkBehaviour
    {
        [SerializeField] private PlayerHitBox[] colliders;
        private RollbackService _rollbackService;
        private PlayerRollbackData[] _playerRollbackData;
        private Vector3[] _colliderPositionsStart;
        private Vector3[] _colliderPositions;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake()
        {
            _colliderPositions = new Vector3[colliders.Length];
            _colliderPositionsStart = new Vector3[colliders.Length];

            for (int i = 0; i < colliders.Length; i++)
            {
                _colliderPositionsStart[i] = colliders[i].transform.localPosition;
            }
        }

        public override void OnNetworkSpawn()
        {
            _playerRollbackData = new PlayerRollbackData[NetworkManager.NetworkTickSystem.TickRate];

            if (!IsOwner) return;

            AddPlayerRollbackServerRpc();

            NetworkManager.NetworkTickSystem.Tick += OnTick;
        }

        [ServerRpc]
        private void AddPlayerRollbackServerRpc()
        {
            _rollbackService.AddPlayer(this, NetworkObject.OwnerClientId);
        }

        private void OnTick()
        {
            for (int i = 0; i < _colliderPositions.Length; i++)
            {
                _colliderPositions[i] = colliders[i].transform.position;
            }

            AddPlayerRollbackDataServerRpc(_colliderPositions);
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void AddPlayerRollbackDataServerRpc(Vector3[] collidersPosition, ServerRpcParams serverRpcParams = default)
        {
            long tick = NetworkManager.NetworkTickSystem.ServerTime.Tick % NetworkManager.NetworkTickSystem.TickRate;
            _playerRollbackData[tick] = new PlayerRollbackData(NetworkManager.NetworkTickSystem.ServerTime.Tick, collidersPosition);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RollbackServerRpc(int tick)
        {
            for (int data = 0; data < _playerRollbackData.Length; data++)
            {
                if (_playerRollbackData[data].Tick == tick)
                {
                    RollbackClientRpc(_playerRollbackData[data]);
                    break;
                }
            }
        }

        [ClientRpc]
        private void RollbackClientRpc(PlayerRollbackData playerRollbackData)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 position = playerRollbackData.ColliderPositions[i];
                colliders[i].transform.localPosition = transform.InverseTransformPoint(position);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReturnServerRpc() => ReturnClientRpc();

        [ClientRpc]
        private void ReturnClientRpc()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].transform.localPosition = _colliderPositionsStart[i];
            }
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}