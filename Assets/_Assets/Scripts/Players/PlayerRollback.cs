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
            Debug.Log($"Current tick: {_rollbackService.CurrentTick}, Data count: {_playerRollbackData.Length}, Last data tick: {_playerRollbackData[^1].Tick}");
            for (int i = 0; i < _playerRollbackData.Length; i++)
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
            long tick = _rollbackService.CurrentTick % NetworkManager.NetworkTickSystem.TickRate;
            _playerRollbackData[tick] = new PlayerRollbackData(position, _rollbackService.CurrentTick);
            //Debug.LogError($"Added Data {serverRpcParams.Receive.SenderClientId}, ServerTick: {_rollbackService.CurrentTick}, Tick: {tick}");
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}