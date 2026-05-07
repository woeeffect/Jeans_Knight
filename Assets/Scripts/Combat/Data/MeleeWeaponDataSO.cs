using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(menuName = "Jeans_Knight/Combat/Melee Weapon", fileName = "MeleeWeapon")]
    public class MeleeWeaponDataSO : WeaponDataSO
    {
        [Header("Melee")]
        [SerializeField, Tooltip("Effective reach of the weapon in meters.")]
        private float _range = 1.8f;

        public float Range => _range;
    }
}
