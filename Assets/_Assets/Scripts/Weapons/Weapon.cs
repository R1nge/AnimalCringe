﻿using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        [SerializeField] protected WeaponConfig weaponConfig;
        [SerializeField] protected NetworkAnimator animator;
        protected NetworkVariable<bool> CanShoot;
        protected float TimeBeforeNextShot;

        private void Awake()
        {
            NetworkManager.NetworkTickSystem.Tick += Tick;
            CanShoot = new NetworkVariable<bool>(true);
        }

        private void Tick() => OnTick();

        protected abstract void OnTick();

        public abstract bool Shoot(ulong owner, Vector3 origin, Vector3 direction, int tick);

        public virtual void Show() => gameObject.SetActive(false);

        public virtual void Hide() => gameObject.SetActive(true);
    }
}