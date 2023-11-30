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
            if (!IsOwner) return;
            NetworkManager.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            for (int i = 0; i < _colliderPositions.Length; i++)
            {
                //TODO: collider local to world position
                _colliderPositions[i] = colliders[i].transform.position;
            }

            AddPlayerRollbackDataServerRpc(_colliderPositions);
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetMouseButtonDown(1))
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
            Debug.LogError("Rollback");
            for (int i = 0; i < colliders.Length; i++)
            {
                Vector3 position = _playerRollbackData[index].Positions[i];
                colliders[i].transform.parent = null;
                colliders[i].transform.position = position;
                StartCoroutine(Return());
            }
        }

        private IEnumerator Return()
        {
            yield return new WaitForSeconds(5f);
            colliders[0].transform.parent = transform;
            colliders[0].transform.localPosition = _colliderPositionsStart[0];
        }

        [ServerRpc]
        private void AddPlayerRollbackDataServerRpc(Vector3[] position, ServerRpcParams serverRpcParams = default)
        {
            if (_playerRollbackData.Count > NetworkManager.NetworkTickSystem.TickRate)
            {
                _playerRollbackData.RemoveAt(0);
            }

            _playerRollbackData.Add(new PlayerRollbackData(position, _rollbackService.CurrentTick));
            Debug.LogError($"Added Data {serverRpcParams.Receive.SenderClientId}, Tick: {_rollbackService.CurrentTick}");
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}