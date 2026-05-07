using UnityEngine;

namespace Combat.Runtime
{
    public static class DamageApplicationHelper
    {
        public static void TryApply(Collider other, Stats.DamageInfo damage, GameObject ignoreOwner)
        {
            GameObject target = other.attachedRigidbody != null
                ? other.attachedRigidbody.gameObject
                : other.gameObject;

            if (ignoreOwner != null && target.transform.IsChildOf(ignoreOwner.transform))
                return;

            if (target.TryGetComponent<IDamageable>(out var damageable) && !damageable.IsDead)
                damageable.ApplyDamage(damage);
        }
    }
}
