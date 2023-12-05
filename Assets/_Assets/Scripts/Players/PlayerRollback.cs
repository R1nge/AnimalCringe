using System;
using System.Linq;
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

        public override void OnNetworkSpawn()
        {
            _playerRollbackData = new PlayerRollbackData[NetworkManager.NetworkTickSystem.TickRate];
            _colliderPositions = new Vector3[colliders.Length];
            _colliderPositionsStart = new Vector3[colliders.Length];

            for (int i = 0; i < colliders.Length; i++)
            {
                _colliderPositionsStart[i] = colliders[i].transform.localPosition;
            }


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
            _playerRollbackData[tick] = new PlayerRollbackData(NetworkManager.NetworkTickSystem.ServerTime.Time, collidersPosition);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RollbackServerRpc(double time)
        {
            //It freezes the position, because it will be fed to the server

            for (int data = 0; data < _playerRollbackData.Length; data++)
            {
                for (int i = 0; i < _playerRollbackData.Length; i++)
                {
                    //TODO: find the closest tick
                    if (Math.Abs(_playerRollbackData[i].Time - time) <= 1f / NetworkManager.NetworkTickSystem.TickRate)
                    {
                        PlayerRollbackData first = _playerRollbackData[i]; //0

                        PlayerRollbackData second = i == 0 ? _playerRollbackData[^1] : _playerRollbackData[i - 1];

                        var delta = (float)(first.Time - second.Time);
                        float t = Mathf.Clamp01((float)(time - second.Time) / delta);

                        Vector3 position = Vector3.Lerp(first.ColliderPositions[0], second.ColliderPositions[0], t);

                        Debug.LogError($"Rollback LERP: {t}");
                        colliders[0].transform.localPosition = transform.InverseTransformPoint(position);

                        break;
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ReturnServerRpc()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].transform.localPosition = _colliderPositionsStart[i];
            }
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}