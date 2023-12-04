using System;
using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Players
{
    public class PlayerRollback : NetworkBehaviour
    {
        private RollbackService _rollbackService;
        private PlayerRollbackData[] _playerRollbackData;
        private CPMPlayer _player;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake()
        {
            _player = GetComponent<CPMPlayer>();
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
            AddPlayerRollbackDataServerRpc(transform.position, transform.rotation);
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void AddPlayerRollbackDataServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
        {
            long tick = NetworkManager.NetworkTickSystem.ServerTime.Tick % NetworkManager.NetworkTickSystem.TickRate;
            _playerRollbackData[tick] = new PlayerRollbackData(NetworkManager.NetworkTickSystem.ServerTime.Tick, position, rotation, _player.Velocity);
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
            Vector3 position = playerRollbackData.Position;
            Quaternion rotation = playerRollbackData.Rotation;
            transform.SetPositionAndRotation(position, rotation);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReturnServerRpc(int tickWhenRollbackHappen)
        {
            long currentTick = NetworkManager.NetworkTickSystem.ServerTime.Tick % NetworkManager.NetworkTickSystem.TickRate;

            long delta = currentTick - tickWhenRollbackHappen;

            for (int data = 0; data < _playerRollbackData.Length; data++)
            {
                if (_playerRollbackData[data].Tick == tickWhenRollbackHappen)
                {
                    int rollbackModulo = tickWhenRollbackHappen % (int) NetworkManager.NetworkTickSystem.TickRate;
                    ResetPlayerPositionClientRpc(_playerRollbackData[rollbackModulo]);
                    for (int i = 0; i < delta; i++)
                    {
                        int moduloTick = (tickWhenRollbackHappen + i) % (int) NetworkManager.NetworkTickSystem.TickRate;
                        Debug.LogError($"[SERVER] MODULO TICK {moduloTick}");
                        ReturnClientRpc(_playerRollbackData[moduloTick]);
                    }

                    break;
                }
            }
        }

        [ClientRpc]
        private void ResetPlayerPositionClientRpc(PlayerRollbackData playerRollbackData)
        {
            Vector3 position = playerRollbackData.Position;
            Quaternion rotation = playerRollbackData.Rotation;
            transform.SetPositionAndRotation(position, rotation);
        }

        [ClientRpc]
        private void ReturnClientRpc(PlayerRollbackData playerRollbackData)
        {
            _player.RollbackSetVelocity(playerRollbackData.Velocity);
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}