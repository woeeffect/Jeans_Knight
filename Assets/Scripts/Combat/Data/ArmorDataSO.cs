using Inventory.Data;
using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(menuName = "Jeans_Knight/Combat/Armor", fileName = "Armor")]
    public class ArmorDataSO : ItemDataSO, IEquippable
    {
        [Header("Armor Visuals")]
        [SerializeField] private GameObject _wornPrefab;

        [Header("Slot")]
        [SerializeField] private EquipmentSlot _slot = EquipmentSlot.Torso;

        [Header("Defense")]
        [SerializeField, Tooltip("Flat physical damage mitigation.")]
        private float _physicalDefense = 0f;
        [SerializeField, Tooltip("Flat magical damage mitigation.")]
        private float _magicDefense = 0f;

        public GameObject WornPrefab => _wornPrefab;
        public EquipmentSlot TargetSlot => _slot;
        public float PhysicalDefense => _physicalDefense;
        public float MagicDefense => _magicDefense;

        private void OnValidate()
        {
            if (_slot == EquipmentSlot.Weapon)
                _slot = EquipmentSlot.Torso;
        }
    }
}
