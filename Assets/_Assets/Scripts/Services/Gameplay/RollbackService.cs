using System.Collections.Generic;
using _Assets.Scripts.Players;
using Unity.Netcode;

namespace _Assets.Scripts.Services.Gameplay
{
    public class RollbackService : NetworkBehaviour
    {
        private NetworkVariable<int> _currentTick;
        public int CurrentTick => _currentTick.Value;
        private readonly List<PlayerRollback> _playerRollbacks = new();

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _currentTick = new NetworkVariable<int>();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerRollbackServerRpc(NetworkBehaviourReference playerRollback)
        {
            if (playerRollback.TryGet(out PlayerRollback playerRollbackComponent))
            {
                _playerRollbacks.Add(playerRollbackComponent);    
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerRollbackServerRpc(NetworkBehaviourReference playerRollback)
        {
            if (playerRollback.TryGet(out PlayerRollback playerRollbackComponent))
            {
                _playerRollbacks.Remove(playerRollbackComponent);
                
            }
        }

        public void Rollback(int tick)
        {
            for (int i = 0; i < _playerRollbacks.Count; i++)
            {
                _playerRollbacks[i].RollbackServerRpc(tick);
            }
        }

        public void Return()
        {
            for (int i = 0; i < _playerRollbacks.Count; i++)
            {
                _playerRollbacks[i].ReturnServerRpc();
            }
        }

        private void OnTick()
        {
            if (!IsServer) return;
            _currentTick.Value++;
        }
    }
}