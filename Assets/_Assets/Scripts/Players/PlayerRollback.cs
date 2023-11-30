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
        private NetworkList<PlayerRollbackData> _playerRollbackData;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake() => _playerRollbackData = new NetworkList<PlayerRollbackData>();

        public override void OnNetworkSpawn() => NetworkManager.NetworkTickSystem.Tick += OnTick;

        private void OnTick()
        {
            if (!IsOwner) return;
            AddPlayerRollbackDataServerRpc(transform.position);
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetMouseButton(1))
            {
                if (IsServer)
                {
                    for (int i = 0; i < _playerRollbackData.Count; i++)
                    {
                        if (_playerRollbackData[i].Tick == _rollbackService.CurrentTick - 128)
                        {
                            RollbackClientRpc(i);
                            break;
                        }
                    }
                }
                else
                {
                    RollbackServerRpc();
                }
            }
        }

        [ServerRpc]
        private void RollbackServerRpc()
        {
            Debug.LogError($"Current tick: {_rollbackService.CurrentTick}, Data count: {_playerRollbackData.Count}, Last data tick: {_playerRollbackData[^1].Tick}");
            for (int i = 0; i < _playerRollbackData.Count; i++)
            {
                if (_playerRollbackData[i].Tick == _rollbackService.CurrentTick - 128)
                {
                    RollbackClientRpc(i);
                    break;
                }
            }
        }

        [ClientRpc]
        private void RollbackClientRpc(int index)
        {
            Vector3 position = _playerRollbackData[index].Position;
            transform.position = position;
        }

        [ServerRpc]
        private void AddPlayerRollbackDataServerRpc(Vector3 position, ServerRpcParams serverRpcParams = default)
        {
            _playerRollbackData.Add(new PlayerRollbackData(position, _rollbackService.CurrentTick));
            //Debug.LogError($"Added Data {serverRpcParams.Receive.SenderClientId}, Tick: {_rollbackService.CurrentTick}");
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}