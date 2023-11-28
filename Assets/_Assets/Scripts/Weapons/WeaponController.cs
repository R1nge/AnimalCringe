using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        //TODO: spawn full hierarchy
        [SerializeField] private NetworkObject weaponParentPrefab;
        [SerializeField] private List<Weapon> weaponPrefabs;
        private Weapon _weapon;
        private int _currentWeaponIndex;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;
            SpawnWeaponServerRpc(_currentWeaponIndex);
        }

        [ServerRpc]
        private void SpawnWeaponServerRpc(int weaponIndex, ServerRpcParams serverRpcParams = default)
        {
            NetworkObject weaponParent = Instantiate(weaponParentPrefab, transform);
            weaponParent.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
            weaponParent.TrySetParent(transform);
            _weapon = Instantiate(weaponPrefabs[weaponIndex], weaponParent.transform);
            _weapon.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
            _weapon.GetComponent<NetworkObject>().TrySetParent(weaponParent);
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

            if (Input.GetMouseButton(0))
            {
                ShootServerRpc();
            }
        }

        [ServerRpc]
        private void ShootServerRpc()
        {
            _weapon.Shoot();
        }
    }
}