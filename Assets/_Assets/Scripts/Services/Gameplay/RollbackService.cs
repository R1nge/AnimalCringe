using Unity.Netcode;

namespace _Assets.Scripts.Services.Gameplay
{
    public class RollbackService : NetworkBehaviour
    {
        private NetworkVariable<int> _currentTick;
        public int CurrentTick => _currentTick.Value;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _currentTick = new NetworkVariable<int>();
        }

        private void OnTick()
        {
            if (!IsServer) return;
            _currentTick.Value++;
        }
    }
}