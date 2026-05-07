using System.Collections.Generic;
using Combat.Data;
using Combat.Events;
using EventBusSystem;
using Inventory.Data;
using Inventory.Runtime;
using UnityEngine;

namespace Combat.Controllers
{
    public class EquipmentController : MonoBehaviour
    {
        private readonly Dictionary<EquipmentSlot, ItemInstance> _slots
            = new Dictionary<EquipmentSlot, ItemInstance>();

        public ItemInstance GetSlot(EquipmentSlot slot)
        {
            _slots.TryGetValue(slot, out var item);
            return item;
        }

        public WeaponDataSO GetEquippedWeapon()
        {
            var instance = GetSlot(EquipmentSlot.Weapon);
            return instance?.Data as WeaponDataSO;
        }

        public float GetTotalDefense(DamageType type)
        {
            float total = 0f;
            foreach (var kvp in _slots)
            {
                if (kvp.Value?.Data is ArmorDataSO armor)
                {
                    total += type == DamageType.Physical
                        ? armor.PhysicalDefense
                        : armor.MagicDefense;
                }
            }
            return total;
        }

        public ItemInstance Equip(ItemDataSO itemData)
        {
            if (itemData is not IEquippable equippable) return null;

            var newInstance = new ItemInstance(itemData);
            EquipmentSlot slot = equippable.TargetSlot;

            _slots.TryGetValue(slot, out var old);
            _slots[slot] = newInstance;

            EventBus.RaiseEvent<IEquipmentChangedHandler>(h =>
                h.OnEquipmentChanged(slot, old, newInstance));

            return old;
        }

        public ItemInstance Unequip(EquipmentSlot slot)
        {
            if (!_slots.TryGetValue(slot, out var old) || old == null) return null;
            _slots[slot] = null;

            EventBus.RaiseEvent<IEquipmentChangedHandler>(h =>
                h.OnEquipmentChanged(slot, old, null));

            return old;
        }
    }
}
