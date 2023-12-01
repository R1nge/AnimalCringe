using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected WeaponConfig weaponConfig;
        [SerializeField] protected Animator animator;
        protected bool CanShoot;
        protected float TimeBeforeNextShot;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
            CanShoot = true;
        }

        private void Tick() => OnTick();

        protected abstract void OnTick();

        public abstract bool Shoot(ulong owner, Vector3 origin, Vector3 direction, bool isServer);

        public void PlayShootAnimation() => animator.SetTrigger("Shooting");

        public virtual void Show() => gameObject.SetActive(false);

        public virtual void Hide() => gameObject.SetActive(true);
    }
}