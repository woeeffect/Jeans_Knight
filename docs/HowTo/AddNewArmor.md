# Как добавить новую броню

## Шаг 1: Создать ArmorDataSO

**ПКМ → Create → Jeans_Knight / Combat / Armor**

| Поле | Описание |
|---|---|
| Display Name | Название в инвентаре |
| Icon | Спрайт-иконка |
| Slot | Слот: Head / Torso / Gloves / Boots |
| Physical Defense | Вычитается из физического урона |
| Magic Defense | Вычитается из магического урона |

---

## Шаг 2: Добавить в инвентарь

Аналогично оружию:
```csharp
InventoryService.PlayerInventory.TryAdd(new ItemInstance(myArmorSO, 1));
```

---

## Как работает защита

`EquipmentController.GetTotalDefense(DamageType)` суммирует защиту по всем надетым слотам.
Вызывается в `HealthComponent.ApplyDamage` — урон уменьшается плоской величиной:

```
finalDamage = max(0, incomingDamage - totalDefense)
```

Файл защиты: `Combat/Controllers/EquipmentController.cs`, метод `GetTotalDefense(DamageType type)`.

---

## Нет визуальных изменений персонажа?

Броня не меняет внешний вид персонажа автоматически. Чтобы это добавить:
1. Добавь поле `GameObject ArmorPrefab` в `ArmorDataSO`.
2. Подпишись на `IEquipmentChangedHandler` в компоненте-рендерере.
3. При смене слота (не Weapon) менять меш/материал.
