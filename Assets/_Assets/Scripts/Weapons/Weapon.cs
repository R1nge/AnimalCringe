using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public abstract class Weapon : NetworkBehaviour
    {
        [SerializeField] protected Transform shootPoint;
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

        public abstract void Shoot();

        public virtual void Show() => gameObject.SetActive(false);

        public virtual void Hide() => gameObject.SetActive(true);
    }
}