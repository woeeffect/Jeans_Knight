using System.Collections.Generic;
using Combat.Controllers;
using EventBusSystem;
using Input;
using Inventory.Data;
using Inventory.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Inventory.UI
{
    public class InventoryPanel : MonoBehaviour, IInputInventoryHandler
    {
        [Header("Panel Root")]
        [SerializeField] private GameObject _panelRoot;

        [Header("Equip Slot Buttons (Head, Torso, Gloves, Boots, Weapon)")]
        [SerializeField] private Button[] _equipSlotButtons = new Button[5];
        [SerializeField] private Text[] _equipSlotLabels = new Text[5];
        [SerializeField] private Image[] _equipSlotIcons = new Image[5];

        [Header("Inventory Grid")]
        [SerializeField] private Transform _inventoryGrid;
        [SerializeField] private Button _inventoryButtonPrefab;

        [Header("References")]
        [SerializeField] private EquipmentController _equipmentController;

        private InventoryService _inventoryService;
        private readonly List<Button> _spawnedButtons = new();

        private static readonly EquipmentSlot[] _slotOrder =
        {
            EquipmentSlot.Head,
            EquipmentSlot.Torso,
            EquipmentSlot.Gloves,
            EquipmentSlot.Boots,
            EquipmentSlot.Weapon
        };

        [Inject]
        public void Construct(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        private void Awake()
        {
            if (_equipmentController == null)
                _equipmentController = FindAnyObjectByType<EquipmentController>();

            SetupEquipSlotButtons();
        }

        private void OnEnable() => EventBus.Subscribe(this);
        private void OnDisable() => EventBus.Unsubscribe(this);

        private void Start()
        {
            _panelRoot?.SetActive(false);
        }

        public void OnOpenInventory()
        {
            if (_panelRoot == null) return;
            bool open = !_panelRoot.activeSelf;
            _panelRoot.SetActive(open);
            if (open) Refresh();
        }

        public void Refresh()
        {
            RefreshEquipSlots();
            RefreshInventoryGrid();
        }

        private void RefreshEquipSlots()
        {
            for (int i = 0; i < _slotOrder.Length; i++)
            {
                EquipmentSlot slot = _slotOrder[i];
                var instance = _equipmentController?.GetSlot(slot);

                if (_equipSlotLabels.Length > i && _equipSlotLabels[i] != null)
                    _equipSlotLabels[i].text = instance?.Data?.DisplayName ?? $"[{slot}]";

                if (_equipSlotIcons.Length > i && _equipSlotIcons[i] != null)
                {
                    _equipSlotIcons[i].enabled = instance?.Data?.Icon != null;
                    if (instance?.Data?.Icon != null)
                        _equipSlotIcons[i].sprite = instance.Data.Icon;
                }
            }
        }

        private void RefreshInventoryGrid()
        {
            foreach (var btn in _spawnedButtons)
                if (btn != null) Destroy(btn.gameObject);
            _spawnedButtons.Clear();

            if (_inventoryButtonPrefab == null || _inventoryGrid == null) return;
            if (_inventoryService == null) return;

            var inventory = _inventoryService.PlayerInventory;
            for (int i = 0; i < inventory.Count; i++)
            {
                var item = inventory.GetAt(i);
                if (item == null) continue;

                Button btn = Instantiate(_inventoryButtonPrefab, _inventoryGrid);
                var label = btn.GetComponentInChildren<Text>();
                if (label != null) label.text = item.Data?.DisplayName ?? "?";

                var icon = btn.transform.Find("Icon")?.GetComponent<Image>();
                if (icon != null && item.Data?.Icon != null)
                    icon.sprite = item.Data.Icon;

                int capturedIndex = i;
                btn.onClick.AddListener(() => OnInventoryItemClicked(capturedIndex));
                _spawnedButtons.Add(btn);
            }
        }

        private void SetupEquipSlotButtons()
        {
            for (int i = 0; i < _equipSlotButtons.Length && i < _slotOrder.Length; i++)
            {
                if (_equipSlotButtons[i] == null) continue;
                int capturedIndex = i;
                _equipSlotButtons[i].onClick.AddListener(() => OnEquipSlotClicked(capturedIndex));
            }
        }

        private void OnInventoryItemClicked(int inventoryIndex)
        {
            if (_equipmentController == null || _inventoryService == null) return;

            var item = _inventoryService.PlayerInventory.GetAt(inventoryIndex);
            if (item?.Data is not IEquippable) return;

            // Equip: move from inventory to equipment slot.
            var displaced = _equipmentController.Equip(item.Data);
            _inventoryService.PlayerInventory.Remove(item);

            // Return displaced item to inventory.
            if (displaced?.Data != null)
                _inventoryService.PlayerInventory.TryAdd(displaced.Data);

            Refresh();
        }

        private void OnEquipSlotClicked(int slotIndex)
        {
            if (_equipmentController == null || _inventoryService == null) return;
            EquipmentSlot slot = _slotOrder[slotIndex];

            var unequipped = _equipmentController.Unequip(slot);
            if (unequipped?.Data != null)
                _inventoryService.PlayerInventory.TryAdd(unequipped.Data);

            Refresh();
        }
    }
}
