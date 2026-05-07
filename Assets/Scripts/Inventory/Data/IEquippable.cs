namespace Inventory.Data
{
    public interface IEquippable
    {
        EquipmentSlot TargetSlot { get; }
    }
}
