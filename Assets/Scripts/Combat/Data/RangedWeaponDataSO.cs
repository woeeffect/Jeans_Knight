using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(menuName = "Jeans_Knight/Combat/Ranged Weapon", fileName = "RangedWeapon")]
    public class RangedWeaponDataSO : WeaponDataSO
    {
        [Header("Ranged")]
        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private float _projectileSpeed = 18f;
        [SerializeField, Tooltip("Lifetime of the projectile in seconds.")]
        private float _projectileLifetime = 3f;
        [SerializeField, Tooltip("Local offset from the muzzle socket where the projectile spawns.")]
        private Vector3 _muzzleOffset = Vector3.zero;

        public GameObject ProjectilePrefab => _projectilePrefab;
        public float ProjectileSpeed => _projectileSpeed;
        public float ProjectileLifetime => _projectileLifetime;
        public Vector3 MuzzleOffset => _muzzleOffset;
    }
}
