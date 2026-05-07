using System.Collections.Generic;
using Combat.Stats;
using UnityEngine;

namespace Combat.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class WeaponHitbox : MonoBehaviour
    {
        private Collider _collider;
        private DamageInfo _pendingDamage;
        private GameObject _ownerRoot;
        private readonly HashSet<GameObject> _alreadyHit = new HashSet<GameObject>();
        private bool _active;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        public void SetOwner(GameObject ownerRoot) => _ownerRoot = ownerRoot;

        public void Activate(DamageInfo info)
        {
            _pendingDamage = info;
            _alreadyHit.Clear();
            _active = true;
            _collider.enabled = true;
        }

        public void Deactivate()
        {
            _active = false;
            _collider.enabled = false;
            _alreadyHit.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_active) return;

            GameObject target = other.attachedRigidbody != null
                ? other.attachedRigidbody.gameObject
                : other.gameObject;

            if (!_alreadyHit.Add(target)) return;

            DamageApplicationHelper.TryApply(other, _pendingDamage, _ownerRoot);
        }
    }
}
