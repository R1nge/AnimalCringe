using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        [SerializeField] protected WeaponConfig weaponConfig;
        [SerializeField] protected Animator animator;
        protected NetworkVariable<bool> CanShoot;
        protected float TimeBeforeNextShot;

        private void Awake()
        {
            NetworkManager.NetworkTickSystem.Tick += Tick;
            CanShoot = new NetworkVariable<bool>(true);
        }

        private void Tick() => OnTick();

        protected abstract void OnTick();

        public abstract void Shoot(ulong owner, Vector3 origin, Vector3 direction);

        public virtual void Show() => gameObject.SetActive(false);

        public virtual void Hide() => gameObject.SetActive(true);
    }
}