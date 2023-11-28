using UnityEngine;

namespace _Assets.Scripts.Weapons
{
    [CreateAssetMenu(fileName = "Weapon Config", menuName = "Configs/Weapon Config")]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] private int damage;
        [SerializeField] private float fireRate;
        
        public int Damage => damage;
        public float FireRate => fireRate;
    }
}