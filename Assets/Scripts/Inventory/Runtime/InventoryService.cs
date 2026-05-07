namespace Inventory.Runtime
{
    public class InventoryService
    {
        public Inventory PlayerInventory { get; }

        public InventoryService(int capacity = 20)
        {
            PlayerInventory = new Inventory(capacity);
        }
    }
}
