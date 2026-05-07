using Combat.Data;
using Combat.Events;
using Combat.Runtime;
using Combat.Stats;
using EventBusSystem;
using Inventory.Data;
using Inventory.Runtime;
using UnityEngine;

namespace Combat.Controllers
{
    public class WeaponHandler : MonoBehaviour, IEquipmentChangedHandler
    {
        [SerializeField] private Transform _weaponSocket;
        [SerializeField] private Transform _muzzleSocket;
        [SerializeField] private WeaponDataSO _initialWeapon;

        private WeaponDataSO _current;
        private GameObject _instanceRoot;
        private WeaponHitbox _hitbox;

        public WeaponDataSO CurrentWeapon => _current;
        public bool HasWeapon => _current != null;

        private void Start()
        {
            if (_initialWeapon != null)
                Equip(_initialWeapon);
        }

        private void OnEnable() => EventBus.Subscribe(this);
        private void OnDisable() => EventBus.Unsubscribe(this);

        public void OnEquipmentChanged(EquipmentSlot slot, ItemInstance oldItem, ItemInstance newItem)
        {
            if (slot != EquipmentSlot.Weapon) return;
            var weapon = newItem?.Data as WeaponDataSO;
            Equip(weapon);
        }

        public void Equip(WeaponDataSO weapon)
        {
            ClearInstance();
            _current = weapon;

            if (weapon == null || _weaponSocket == null)
                return;

            if (weapon.HeldPrefab != null)
            {
                _instanceRoot = Instantiate(weapon.HeldPrefab, _weaponSocket);
                _instanceRoot.transform.localPosition = Vector3.zero;
                _instanceRoot.transform.localRotation = Quaternion.identity;

                _hitbox = _instanceRoot.GetComponentInChildren<WeaponHitbox>();
                _hitbox?.SetOwner(gameObject);
            }
        }

        public void Unequip()
        {
            ClearInstance();
            _current = null;
        }

        public void HandleHitPhase(DamageInfo damage)
        {
            if (_current is MeleeWeaponDataSO)
                _hitbox?.Activate(damage);
            else if (_current is RangedWeaponDataSO)
                SpawnProjectile(damage);
        }

        public void HandleHitEnd()
        {
            _hitbox?.Deactivate();
        }

        public void ActivateMeleeHitbox(DamageInfo damage)
        {
            if (_current is not MeleeWeaponDataSO) return;
            _hitbox?.Activate(damage);
        }

        public void DeactivateMeleeHitbox()
        {
            _hitbox?.Deactivate();
        }

        public void SpawnProjectile(DamageInfo damage)
        {
            if (_current is not RangedWeaponDataSO ranged) return;
            if (ranged.ProjectilePrefab == null) return;

            Transform origin = _muzzleSocket != null ? _muzzleSocket : _weaponSocket != null ? _weaponSocket : transform;
            Vector3 spawnPos = origin.position + origin.TransformVector(ranged.MuzzleOffset);
            Vector3 direction = origin.forward;

            GameObject go = Instantiate(ranged.ProjectilePrefab, spawnPos, Quaternion.LookRotation(direction));
            if (go.TryGetComponent<Projectile>(out var projectile))
                projectile.Launch(damage, gameObject, direction, ranged.ProjectileSpeed, ranged.ProjectileLifetime);
        }

        private void ClearInstance()
        {
            if (_instanceRoot != null)
                Destroy(_instanceRoot);
            _instanceRoot = null;
            _hitbox = null;
        }
    }
}
