using Combat.Data;
using UnityEngine;

namespace Combat.Stats
{
    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public WeaponCategory WeaponCategory;
        public GameObject Attacker;
        public int ComboIndex;

        public DamageInfo(float amount, DamageType type, WeaponCategory weaponCategory, GameObject attacker, int comboIndex)
        {
            Amount = amount;
            Type = type;
            WeaponCategory = weaponCategory;
            Attacker = attacker;
            ComboIndex = comboIndex;
        }
    }
}
