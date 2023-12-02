using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected LayerMask ignoreLayer;
        [SerializeField] protected WeaponConfig weaponConfig;
        [SerializeField] protected Animator animator;
        protected bool CanShoot;
        protected float TimeBeforeNextShot;

        protected virtual void Awake() => CanShoot = true;

        public abstract void OnTick();

        public abstract HitInfo Shoot(ulong owner, Vector3 origin, Vector3 direction, bool isServer);

        public void PlayShootAnimation() => animator.SetTrigger("Shooting");

        public virtual void Show() => gameObject.SetActive(false);

        public virtual void Hide() => gameObject.SetActive(true);
    }
}