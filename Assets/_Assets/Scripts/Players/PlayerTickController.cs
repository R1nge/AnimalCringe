using _Assets.Scripts.Weapons;
using Unity.Netcode;

namespace _Assets.Scripts.Players
{
    public class PlayerTickController : NetworkBehaviour
    {
        private PlayerRollback _playerRollback;
        private WeaponController _weaponController;
        private CPMPlayer _playerMovement;

        private void Awake()
        {
            _playerRollback = GetComponent<PlayerRollback>();
            _weaponController = GetComponent<WeaponController>();
            _playerMovement = GetComponent<CPMPlayer>();
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
        {
            _playerMovement.OnTick();
            _playerRollback.OnTick();
            _weaponController.OnTick();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            NetworkManager.Singleton.NetworkTickSystem.Tick -= OnTick;
        }
    }
}