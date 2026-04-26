# InventoryPanel

**Путь**: `Assets/Scripts/Inventory/UI/InventoryPanel.cs`
**Namespace**: `Inventory.UI`
**На объекте**: Canvas → InventoryPanel GameObject в сцене

## Ответственность

UI инвентаря: 5 слотов экипировки + grid предметов. Открывается/закрывается по клавише I. Клик на предмет в grid — экипирует его; клик на слот экипировки — снимает предмет обратно в инвентарь.

## Требования к сцене

1. GameObject с компонентом `InventoryPanel`.
2. **`ZenjectBinding`** на том же GameObject (иначе `[Inject]` не сработает).
3. В Inspector заполнить:
   - `panelRoot` — корневой GameObject панели (скрывается/показывается).
   - `_equipSlotButtons[5]` — 5 кнопок слотов (порядок: Head, Torso, Gloves, Boots, Weapon).
   - `_equipSlotLabels[5]` — текстовые метки для каждого слота.
   - `_equipSlotIcons[5]` — иконки экипированных предметов.
   - `_inventoryGrid` — Transform контейнера для grid-кнопок.
   - `_inventoryButtonPrefab` — префаб кнопки предмета (нужен `Text` и опционально `Icon: Image`).
   - `_equipmentController` — компонент [[Classes/Combat/Controllers/EquipmentController\|EquipmentController]] (авто-поиск если пусто).

## Открытие/закрытие

`OnOpenInventory()` из `IInputInventoryHandler` — toggle по кнопке I ([[Systems/Input\|InputService]] → [[Systems/EventBus\|EventBus]]).

## Поток «экипировать предмет»

```
Клик на предмет в grid
  └─ OnInventoryItemClicked(index)
     ├─ item = PlayerInventory.GetAt(index)
     ├─ item.Data is IEquippable? → нет → ничего
     ├─ displaced = EquipmentController.Equip(item.Data)   ← вытесняет старое
     ├─ PlayerInventory.Remove(item)                        ← убрать из инвентаря
     ├─ PlayerInventory.TryAdd(displaced.Data)             ← вернуть вытесненное
     └─ Refresh()                                          ← перерисовать UI
```

## Поток «снять предмет»

```
Клик на слот экипировки
  └─ OnEquipSlotClicked(slotIndex)
     ├─ unequipped = EquipmentController.Unequip(slot)
     ├─ PlayerInventory.TryAdd(unequipped.Data)
     └─ Refresh()
```

## Что изменить здесь

| Задача | Где |
|---|---|
| Добавить drag-and-drop | Переписать `OnInventoryItemClicked` с `IBeginDragHandler` |
| Показать количество предметов | Добавить поле `Count` в текст кнопки grid |
| Добавить подсветку при наведении | `IPointerEnterHandler` на prefab кнопки |
| Изменить количество слотов | Расширить `SlotOrder` массив + `EquipmentSlot` enum |

## Связанные файлы

- [[InventoryService]] — откуда берётся `PlayerInventory`
- [[Classes/Combat/Controllers/EquipmentController\|EquipmentController]] — экипировка/снятие
- [[DataObjects/WeaponDataSO]] / `ArmorDataSO` — должны реализовывать `IEquippable`
