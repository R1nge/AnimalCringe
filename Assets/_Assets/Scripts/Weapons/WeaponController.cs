using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public class WeaponController : NetworkBehaviour
    {
        [SerializeField] private Transform weaponParent;
        [SerializeField] private List<Weapon> weaponPrefabs;
        private Weapon _weapon;
        private int _currentWeaponIndex;

        public override void OnNetworkSpawn()
        {
            _weapon = Instantiate(weaponPrefabs[_currentWeaponIndex], weaponParent);
            _weapon.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _weapon.Shoot();
            }

            _weapon.transform.position = weaponParent.position;
        }
    }
}