using System.Collections.Generic;
using Inventory.Data;
using UnityEngine;

namespace Combat.Data
{
    public abstract class WeaponDataSO : ItemDataSO, IEquippable
    {
        [Header("Weapon Visuals")]
        [SerializeField] private GameObject _heldPrefab;

        [Header("Classification")]
        [SerializeField] private WeaponCategory _category;
        [SerializeField] private DamageType _damageType;

        [Header("Stats")]
        [SerializeField] private float _baseDamage = 10f;
        [SerializeField] private float _staminaCost = 10f;

        [Header("Combo")]
        [SerializeField, Tooltip("Max seconds between hits to stay in the combo chain.")]
        private float _comboWindow = 0.8f;
        [SerializeField, Tooltip("Attack chain. Combo cycles through these in order.")]
        private List<AttackActionSO> _combo = new List<AttackActionSO>();

        public GameObject HeldPrefab => _heldPrefab;
        public WeaponCategory Category => _category;
        public DamageType DamageType => _damageType;
        public float BaseDamage => _baseDamage;
        public float StaminaCost => _staminaCost;
        public float ComboWindow => _comboWindow;
        public IReadOnlyList<AttackActionSO> Combo => _combo;

        public EquipmentSlot TargetSlot => EquipmentSlot.Weapon;
    }
}
