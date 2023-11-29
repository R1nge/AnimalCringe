using Unity.Netcode;

namespace _Assets.Scripts.Players
{
    public class PlayerInput : NetworkBehaviour
    {
        private readonly NetworkVariable<bool> _enabled = new(true);
        public bool Enabled => _enabled.Value;

        [ServerRpc(RequireOwnership = false)]
        public void EnableServerRpc(bool enable) => _enabled.Value = enable;
    }
}