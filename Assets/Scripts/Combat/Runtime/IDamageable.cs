using Combat.Stats;
using UnityEngine;

namespace Combat.Runtime
{
    public interface IDamageable
    {
        GameObject GameObject { get; }
        bool IsDead { get; }
        void ApplyDamage(DamageInfo info);
    }
}
