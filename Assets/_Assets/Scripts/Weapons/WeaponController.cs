using System;
using System.Collections;
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

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        public override void OnNetworkSpawn() => _weapon = weapons[_currentWeaponIndex];

        public void OnTick()
        {
            if (!IsOwner) return;
            if (!_playerInput.Enabled) return;
            if (_weapon == null) return;

            _weapon.OnTick();
            
           
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!_playerInput.Enabled) return;
            if (_weapon == null) return;

            if (Input.GetMouseButton(0))
            {
                Vector3 shootOrigin = playerCamera.transform.position;
                Vector3 shootDirection = playerCamera.transform.forward;

                double shootTime = NetworkManager.Singleton.NetworkTickSystem.ServerTime.Time;
                
                Debug.LogError($"[SERVER] SHOOT TIME {shootTime}");
                Debug.LogError($"[CLIENT] SHOOT TIME {NetworkManager.Singleton.NetworkTickSystem.LocalTime.Time}");

                if (!IsServer)
                {
                    PlayAnimationServerRpc();
                    ShootServerRpc(OwnerClientId, shootOrigin, shootDirection, shootTime);
                }
                else
                {
                    _weapon.Shoot(OwnerClientId, shootOrigin, shootDirection);
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(ulong ownerId, Vector3 shootOrigin, Vector3 shootDirection, double shootTime)
        {
            _rollbackService.Rollback(shootTime);
            _weapon.Shoot(ownerId, shootOrigin, shootDirection);
            _rollbackService.Return();
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