using System;
using System.Collections;
using System.Collections.Generic;
using _Assets.Scripts.Services.Gameplay;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Players
{
    public class PlayerRollback : NetworkBehaviour
    {
        [SerializeField] private Collider[] colliders;
        private RollbackService _rollbackService;
        private List<PlayerRollbackData> _playerRollbackData;
        private Vector3[] _colliderPositionsStart;
        private Vector3[] _colliderPositions;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake()
        {
            _playerRollbackData = new List<PlayerRollbackData>();
            _colliderPositions = new Vector3[colliders.Length];
            _colliderPositionsStart = new Vector3[colliders.Length];

            for (int i = 0; i < colliders.Length; i++)
            {
                _colliderPositionsStart[i] = colliders[i].transform.localPosition;
            }
        }

        public override void OnNetworkSpawn()
        {
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
            Debug.Log($"Current tick: {_rollbackService.CurrentTick}, Data count: {_playerRollbackData.Count}, Last data tick: {_playerRollbackData[^1].Tick}");
            for (int i = 0; i < _playerRollbackData.Count; i++)
            {
                if (_playerRollbackData[i].Tick == tick)
                {
                    RollbackClientRpc(_playerRollbackData[i]);
                    break;
                }
            }
        }

        [ClientRpc]
        private void RollbackClientRpc(PlayerRollbackData playerRollbackData)
        {
            Debug.LogError("Rollback");
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 position = playerRollbackData.Positions[i];
                colliders[i].transform.parent = null;
                colliders[i].transform.position = position;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReturnServerRpc() => ReturnClientRpc();

        [ClientRpc]
        private void ReturnClientRpc()
        {
            colliders[0].transform.parent = transform;
            colliders[0].transform.localPosition = _colliderPositionsStart[0];
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void AddPlayerRollbackDataServerRpc(Vector3[] position, ServerRpcParams serverRpcParams = default)
        {
            if (_playerRollbackData.Count > NetworkManager.NetworkTickSystem.TickRate)
            {
                _playerRollbackData.RemoveAt(0);
            }

            _playerRollbackData.Add(new PlayerRollbackData(position, _rollbackService.CurrentTick));
            Debug.Log($"Added Data {serverRpcParams.Receive.SenderClientId}, Tick: {_rollbackService.CurrentTick}");
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}