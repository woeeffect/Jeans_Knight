using Combat.Data;
using Combat.Stats;
using UnityEngine;

namespace Combat.Runtime
{
    public static class DamageCalculator
    {
        private const float _comboStepMultiplier = 1.15f;

        public static DamageInfo Compute(
            WeaponDataSO weapon,
            AttackActionSO action,
            CharacterStatsSO attackerStats,
            int comboIndex,
            GameObject attacker)
        {
            float stat = weapon.DamageType == DamageType.Magic
                ? attackerStats.MagicPower
                : attackerStats.Strength;

            float statMult = 1f + stat * attackerStats.StatScaling;
            float comboMult = Mathf.Pow(_comboStepMultiplier, Mathf.Max(0, comboIndex));
            float actionMult = action != null ? action.DamageMultiplier : 1f;

            float amount = weapon.BaseDamage * statMult * comboMult * actionMult;

            return new DamageInfo(
                amount,
                weapon.DamageType,
                weapon.Category,
                attacker,
                comboIndex);
        }
    }
}
