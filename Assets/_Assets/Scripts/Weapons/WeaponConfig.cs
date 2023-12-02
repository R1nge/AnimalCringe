using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Config", menuName = "Configs/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private int damage;
        [SerializeField] private float fireRate;
        [SerializeField] private float range;
        
        public int Damage => damage;
        public float FireRate => fireRate;
        public float Range => range;
    }
}