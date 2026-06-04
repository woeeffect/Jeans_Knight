# EquipmentController

**Путь**: `Assets/Scripts/Combat/Controllers/EquipmentController.cs`
**Namespace**: `Combat.Controllers`

## Ответственность

Хранит словарь `EquipmentSlot → ItemInstance`. Одна ответственность — «что надето на персонаже прямо сейчас».

## Методы

| Метод | Возвращает | Описание |
|---|---|---|
| `Equip(ItemDataSO item)` | `ItemInstance` (вытесненный) | Надевает предмет, снимает старый |
| `Unequip(EquipmentSlot slot)` | `ItemInstance` (снятый) | Снимает предмет со слота |
| `GetEquipped(EquipmentSlot slot)` | `ItemInstance` или null | Что надето в слоте |
| `GetTotalDefense(DamageType type)` | `float` | Суммарная защита от конкретного типа урона |

## Что делает Equip()

1. Проверяет `item as IEquippable` → получает `TargetSlot`.
2. Если в слоте уже что-то есть — снимает (сохраняет для возврата).
3. Кладёт новый предмет в слот.
4. `EventBus.RaiseEvent<IEquipmentChangedHandler>(...)`.

## Связь с WeaponHandler

`WeaponHandler` подписан на `IEquipmentChangedHandler`.
При смене слота `Weapon` — автоматически пересобирает префаб оружия в руке.

## Защита от урона

```csharp
float defense = GetTotalDefense(DamageType.Physical);
// Суммирует ArmorDataSO.PhysicalDefense по всем слотам (Head, Torso, Gloves, Boots)
// Слот Weapon игнорируется при подсчёте защиты
```

Вызывается в `HealthComponent.ApplyDamage`.
