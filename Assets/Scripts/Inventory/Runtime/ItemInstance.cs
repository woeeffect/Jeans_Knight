using Inventory.Data;

namespace Inventory.Runtime
{
    public class ItemInstance
    {
        public ItemDataSO Data { get; }
        public int Count { get; set; }

        public ItemInstance(ItemDataSO data, int count = 1)
        {
            Data = data;
            Count = count;
        }
    }
}
