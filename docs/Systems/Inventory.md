# Инвентарь и экипировка

## Файлы

| Файл | Роль |
|---|---|
| `Inventory/Data/ItemDataSO.cs` | Базовый класс для всех предметов |
| `Inventory/Data/EquipmentSlot.cs` | Enum: Head, Torso, Gloves, Boots, Weapon |
| `Inventory/Data/IEquippable.cs` | Интерфейс: `EquipmentSlot TargetSlot { get; }` |
| `Inventory/Runtime/ItemInstance.cs` | Рантайм-обёртка: `ItemDataSO Data`, `int Count` |
| `Inventory/Runtime/Inventory.cs` | Список предметов с ёмкостью |
| `Inventory/Runtime/InventoryService.cs` | Zenject-сервис: единая точка для UI и кода |
| `Inventory/UI/InventoryPanel.cs` | UI-панель, открывается по I |
| `Combat/Controllers/EquipmentController.cs` | Словарь слот→ItemInstance на персонаже |

---

## Как работает экипировка

```
Игрок нажимает I
  └─ InventoryPanel открывается (IInputInventoryHandler.OnOpenInventory)

Игрок кликает на предмет в инвентаре
  └─ InventoryService.Equip(itemInstance)
     └─ EquipmentController.Equip(itemData)
        ├─ slot = (itemData as IEquippable).TargetSlot
        ├─ старый предмет возвращается в инвентарь
        └─ EventBus.RaiseEvent<IEquipmentChangedHandler>(...)

WeaponHandler (слушает IEquipmentChangedHandler)
  └─ OnEquipmentChanged(slot, oldItem, newItem)
     └─ если slot == Weapon:
        ├─ Destroy(старый префаб в руке)
        └─ Instantiate(новый HeldPrefab под weaponSocket)
```

---

## Иерархия данных предметов

```
ScriptableObject
    └─ ItemDataSO (abstract)  — displayName, icon, maxStack
           ├─ WeaponDataSO (abstract) + IEquippable — BaseDamage, Category, Combo...
           │       ├─ MeleeWeaponDataSO         — + range
           │       └─ RangedWeaponDataSO         — + projectilePrefab, speed...
           └─ ArmorDataSO + IEquippable          — slot, physicalDefense, magicDefense
```

---

## EquipmentController — защита броней

`EquipmentController.GetTotalDefense(DamageType)` суммирует защиту всех надетых брон по типу урона:
- `DamageType.Physical` → суммирует `ArmorDataSO.PhysicalDefense` со всех слотов
- `DamageType.Magic` → суммирует `ArmorDataSO.MagicDefense`

Вызывается в `HealthComponent.ApplyDamage` перед применением урона.

---

## InventoryService (Zenject Single)

Инжектируется в `InventoryPanel` через `[Inject]`.
Держит `PlayerInventory` (размер N, задаётся в инспекторе) и ссылку на `EquipmentController`.

> **Сцена**: на GameObject панели инвентаря должен быть компонент **ZenjectBinding**, биндящий `InventoryPanel` — иначе `[Inject]` не сработает.
