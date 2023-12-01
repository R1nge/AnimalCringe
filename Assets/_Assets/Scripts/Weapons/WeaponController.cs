using System;
using System.Collections.Generic;
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

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _weapon = weapons[_currentWeaponIndex];
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!_playerInput.Enabled) return;

            if (Input.GetMouseButton(0))
            {
                Vector3 shootOrigin = playerCamera.transform.position;
                Vector3 shootDirection = playerCamera.transform.forward;

                if (_weapon.Shoot(OwnerClientId, shootOrigin, shootDirection, false))
                {
                    ShootServerRpc(OwnerClientId, shootOrigin, shootDirection);
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(ulong clientId, Vector3 position, Vector3 direction)
        {
            _rollbackService.Rollback(_rollbackService.CurrentTick - 128);
            _weapon.Shoot(clientId, position, direction, true);
            _rollbackService.Return();
        }
    }
}