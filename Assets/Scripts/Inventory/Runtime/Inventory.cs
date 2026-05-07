using System.Collections.Generic;
using Inventory.Data;

namespace Inventory.Runtime
{
    public class Inventory
    {
        private readonly List<ItemInstance> _items;
        private readonly int _capacity;

        public IReadOnlyList<ItemInstance> Items => _items;
        public int Capacity => _capacity;
        public int Count => _items.Count;

        public Inventory(int capacity = 20)
        {
            _capacity = capacity;
            _items = new List<ItemInstance>(capacity);
        }

        public bool TryAdd(ItemDataSO data, int count = 1)
        {
            if (_items.Count >= _capacity) return false;
            _items.Add(new ItemInstance(data, count));
            return true;
        }

        public bool Remove(ItemInstance instance)
        {
            return _items.Remove(instance);
        }

        public ItemInstance GetAt(int index)
        {
            if (index < 0 || index >= _items.Count) return null;
            return _items[index];
        }
    }
}
