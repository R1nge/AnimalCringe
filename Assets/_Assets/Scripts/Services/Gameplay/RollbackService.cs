using System.Collections.Generic;
using _Assets.Scripts.Players;
using Unity.Netcode;

namespace _Assets.Scripts.Services.Gameplay
{
    //TODO: To do it right you would have to roll back all the players then do the raycast and resimulate it
    public class RollbackService : NetworkBehaviour
    {
        private int _currentTick;
        public int CurrentTick => _currentTick;
        private readonly Dictionary<ulong, PlayerRollback> _playerRollbacks = new();

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            if (IsServer)
            {
                RemovePlayer(clientId);
            }
        }

        public void AddPlayer(NetworkBehaviourReference playerRollback, ulong clientId)
        {
            if (playerRollback.TryGet(out PlayerRollback playerRollbackComponent))
            {
                _playerRollbacks.Add(clientId, playerRollbackComponent);
            }
        }

        private void RemovePlayer(ulong clientId) => _playerRollbacks.Remove(clientId);

        public void Rollback(int tick)
        {
            foreach (var keyValuePair in _playerRollbacks)
            {
                _playerRollbacks[keyValuePair.Key].RollbackServerRpc(tick);
            }
        }

        public void Return()
        {
            foreach (var keyValuePair in _playerRollbacks)
            {
                _playerRollbacks[keyValuePair.Key].ReturnServerRpc();
            }
        }

        private void OnTick()
        {
            if (!IsServer) return;
            _currentTick++;
        }
    }
}