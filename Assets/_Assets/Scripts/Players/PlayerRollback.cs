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
        }

        [ServerRpc]
        private void AddPlayerRollbackServerRpc()
        {
            _rollbackService.AddPlayer(this, NetworkObject.OwnerClientId);
        }

        public void OnTick()
        {
            //Set the previous positon data on rollback

            for (int i = 0; i < _colliderPositions.Length; i++)
            {
                _colliderPositions[i] = colliders[i].transform.position;
            }

            AddPlayerRollbackDataServerRpc(_colliderPositions);
        }

        [ServerRpc]
        private void AddPlayerRollbackDataServerRpc(Vector3[] collidersPosition)
        {
            long tick = NetworkManager.NetworkTickSystem.ServerTime.Tick % NetworkManager.NetworkTickSystem.TickRate;
            _playerRollbackData[tick] = new PlayerRollbackData(NetworkManager.NetworkTickSystem.ServerTime.Time, collidersPosition);
        }

        public void Rollback(double time)
        {
            //I can find the max value firts
            //Theen loop backwards

            int currentIndex = Array.FindIndex(_playerRollbackData, data => Math.Abs(data.Time - time) < 1f / NetworkManager.NetworkTickSystem.TickRate);
            int previousIndex = (currentIndex - 1 + _playerRollbackData.Length) % _playerRollbackData.Length;

            PlayerRollbackData current = _playerRollbackData[currentIndex];
            PlayerRollbackData previous = _playerRollbackData[previousIndex];

            Debug.LogError($"TIME {time} CURRENT TIME {current.Time}; PREVIOUS TIME {previous.Time}; MAX TIME {_playerRollbackData.Max(data => data.Time)}");

            //Example:
            //time = 27.049998826459
            //current tick time = 27.04
            //previous tick time = 27.02
            //delta between time and current.Time = 0.009998
            //delta between ticks = 0.01771
            //interpolation = 0.435461

            double delta = time - current.Time;
            double tickDelta = current.Time - previous.Time;
            double interpolation = Clamp01(delta / tickDelta);

            var newInterpolation = interpolation;//1 - interpolation;

            //Vector3 position = Vector3.Lerp(previous.ColliderPositions[0], current.ColliderPositions[0], interpolation);
            Vector3 position = Vector3.Lerp(previous.ColliderPositions[0], current.ColliderPositions[0], (float)newInterpolation);

            Debug.LogError($"Rollback LERP: {interpolation}");

            colliders[0].transform.localPosition = transform.TransformPoint(position);
        }

        private double Clamp01(double value)
        {
            if (value < 0.0)
                return 0.0d;
            return value > 1.0 ? 1d : value;
        }

        public void Return()
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].transform.localPosition = _colliderPositionsStart[i];
            }
        }

        public override void OnNetworkDespawn() => NetworkManager.NetworkTickSystem.Tick -= OnTick;
    }
}