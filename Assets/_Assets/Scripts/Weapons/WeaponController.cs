using System.Collections.Generic;
using System.Linq;
using _Assets.Scripts.Players;
using _Assets.Scripts.Services.Gameplay;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private List<Weapon> weapons;
        private Weapon _weapon;
        private int _currentWeaponIndex;
        private PlayerInput _playerInput;
        private RollbackService _rollbackService;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        public override void OnNetworkSpawn() => _weapon = weapons[_currentWeaponIndex];

        private void Update()
        {
            if (!IsOwner) return;
            if (!_playerInput.Enabled) return;

            if (Input.GetMouseButton(0))
            {
                Vector3 shootOrigin = playerCamera.transform.position;
                Vector3 shootDirection = playerCamera.transform.forward;

                PlayAnimationServerRpc();
                
                if (IsServer)
                {
                    _weapon.Shoot(OwnerClientId, shootOrigin, shootDirection, true);
                    return;
                }

                HitInfo hitInfo = _weapon.Shoot(OwnerClientId, shootOrigin, shootDirection, false);

                if (hitInfo.Hit)
                {
                    ShootServerRpc(OwnerClientId, hitInfo.VictimId, shootOrigin, shootDirection);
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(ulong ownerId, ulong victimId, Vector3 position, Vector3 direction)
        {
            Debug.LogError($"Hit Owner {ownerId}, Victim {victimId}");
            _rollbackService.Rollback(victimId, _rollbackService.CurrentTick);
            _weapon.Shoot(ownerId, position, direction, true);
            _rollbackService.Return(victimId);
        }

        [ServerRpc]
        private void PlayAnimationServerRpc()
        {
            _weapon.PlayShootAnimation();
            PlayAnimationClientRpc();
        }

        [ClientRpc]
        private void PlayAnimationClientRpc() => _weapon.PlayShootAnimation();
    }

    public struct ShootInfo
    {
        private ulong _ownerId;
        private Vector3 _shootOrigin;
        private Vector3 _shootDirection;
        private bool _isServer;
    }

    public struct HitInfo
    {
        public bool Hit;
        public ulong VictimId;
    }
}