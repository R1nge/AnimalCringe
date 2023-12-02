using System.Collections.Generic;
using _Assets.Scripts.Players;
using Unity.Netcode;

namespace _Assets.Scripts.Services.Gameplay
{
    public class RollbackService : NetworkBehaviour
    {
        private int _currentTick;
        public int CurrentTick => _currentTick;
        private readonly Dictionary<ulong, PlayerRollback> _playerRollbacks = new();

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        }

        public void AddPlayer(NetworkBehaviourReference playerRollback, ulong clientId)
        {
            if (playerRollback.TryGet(out PlayerRollback playerRollbackComponent))
            {
                _playerRollbacks.Add(clientId, playerRollbackComponent);
            }
        }

        public void RemovePlayer(ulong clientId)
        {
            _playerRollbacks.Remove(clientId);
        }

        public void Rollback(ulong clientId, int tick)
        {
            _playerRollbacks[clientId].RollbackServerRpc(tick);
        }

        public void Return(ulong clientId)
        {
            _playerRollbacks[clientId].ReturnServerRpc();
        }

        private void OnTick()
        {
            if (!IsServer) return;
            _currentTick++;
        }
    }
}