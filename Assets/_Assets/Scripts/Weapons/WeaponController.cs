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

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        public override void OnNetworkSpawn()
        {
            _weapon = weapons[_currentWeaponIndex];
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
        }

        private void OnTick()
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

                if (!IsServer)
                {
                    PlayAnimationServerRpc();
                    //So, the game state should be rolled back to the shooter local tick
                    ShootServerRpc(OwnerClientId, shootOrigin, shootDirection, NetworkManager.Singleton.NetworkTickSystem.LocalTime.Tick);
                }
                else
                {
                    _weapon.Shoot(OwnerClientId, shootOrigin, shootDirection);
                }
            }
        }

        public void RemoveWeapons()
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                Destroy(weapons[i]);
            }

            weapons.Clear();
            _weapon = null;
        }

        [ServerRpc]
        private void ShootServerRpc(ulong ownerId, Vector3 shootOrigin, Vector3 shootDirection, int tick)
        {
            _rollbackService.Rollback(tick);
            //Since I'm rolling back colliders, colliders should be damageable
            _weapon.Shoot(ownerId, shootOrigin, shootDirection);
            //TODO: resimulate everything to this tick
            //So, move the player to the shot tick, then do the raycast, apply all of the inputs to this tick
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