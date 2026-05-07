using EventBusSystem;
using Inventory.Data;
using Inventory.Runtime;

namespace Combat.Events
{
    public interface IEquipmentChangedHandler : IGlobalSubscriber
    {
        void OnEquipmentChanged(EquipmentSlot slot, ItemInstance oldItem, ItemInstance newItem);
    }
}
