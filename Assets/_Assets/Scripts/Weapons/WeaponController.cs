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
        [SerializeField] private NetworkObject weaponParentPrefab;
        [SerializeField] private List<Weapon> weaponPrefabs;
        private Weapon _weapon;
        private int _currentWeaponIndex;
        private PlayerInput _playerInput;
        private RollbackService _rollbackService;

        [Inject]
        private void Inject(RollbackService rollbackService) => _rollbackService = rollbackService;

        private void Awake() => _playerInput = GetComponent<PlayerInput>();

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            StartCoroutine(SpawnWeapons_C());
        }

        private IEnumerator SpawnWeapons_C()
        {
            yield return null;
            SpawnWeaponServerRpc(_currentWeaponIndex);
        }

        [ServerRpc]
        private void SpawnWeaponServerRpc(int weaponIndex, ServerRpcParams serverRpcParams = default)
        {
            NetworkObject weaponParent = Instantiate(weaponParentPrefab, transform.position, Quaternion.identity);
            weaponParent.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
            weaponParent.TrySetParent(transform);
            _weapon = Instantiate(weaponPrefabs[weaponIndex], weaponParent.transform);
            _weapon.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
            _weapon.GetComponent<NetworkObject>().TrySetParent(weaponParent);
            _weapon.transform.localPosition = Vector3.zero;
            AssignWeaponClientRpc(_weapon.GetComponent<NetworkObject>());
        }

        [ClientRpc]
        private void AssignWeaponClientRpc(NetworkObjectReference networkObjectReference)
        {
            if (networkObjectReference.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out Weapon weapon))
                {
                    _weapon = weapon;
                }
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!_playerInput.Enabled) return;

            if (Input.GetMouseButton(0))
            {
                if (_weapon.Shoot(OwnerClientId, playerCamera.transform.position, playerCamera.transform.forward))
                {
                    ShootServerRpc();
                }
            }
        }

        [ServerRpc]
        private void ShootServerRpc(ServerRpcParams rpcParams = default)
        {
            _rollbackService.Rollback(_rollbackService.CurrentTick);
            _weapon.Shoot(rpcParams.Receive.SenderClientId, playerCamera.transform.position, playerCamera.transform.forward);
            StartCoroutine(Return());
        }

        private IEnumerator Return()
        {
            yield return null;
            _rollbackService.Return();
        }
    }
}