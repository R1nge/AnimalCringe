using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Players
{
    public class PlayerRollback : NetworkBehaviour
    {
        [SerializeField] private Collider[] colliders;
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

            if (IsServer)
            {
                _rollbackService.AddPlayerRollbackServerRpc(this);
            }

            if (!IsOwner) return;

            NetworkManager.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            for (int i = 0; i < _colliderPositions.Length; i++)
            {
                _colliderPositions[i] = colliders[i].transform.position;
            }

            AddPlayerRollbackDataServerRpc(_colliderPositions);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RollbackServerRpc(int tick)
        {
            for (int data = 0; data < _playerRollbackData.Length; data++)
            {
                for (int collider = 0; collider < colliders.Length; collider++)
                {
                    Vector3 position = _playerRollbackData[data].Positions[collider];
                    colliders[collider].transform.localPosition = transform.InverseTransformPoint(position);
                }

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
                Vector3 position = playerRollbackData.Positions[i];
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
                colliders[i].transform.localPosition = _colliderPositionsStart[0];
            }
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void AddPlayerRollbackDataServerRpc(Vector3[] position, ServerRpcParams serverRpcParams = default)
        {
            long tick = _rollbackService.CurrentTick % NetworkManager.NetworkTickSystem.TickRate;
            _playerRollbackData[tick] = new PlayerRollbackData(position, _rollbackService.CurrentTick);
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}