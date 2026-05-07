using UnityEngine;

namespace Inventory.Data
{
    public abstract class ItemDataSO : ScriptableObject
    {
        [SerializeField] protected string displayName;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected int maxStack = 1;

        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public int MaxStack => maxStack;
    }
}
