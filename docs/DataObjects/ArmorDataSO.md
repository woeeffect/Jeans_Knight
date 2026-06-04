# ArmorDataSO

**Путь**: `Assets/Scripts/Combat/Data/ArmorDataSO.cs`
[Create → Jeans_Knight/Combat/Armor]

## Иерархия

```
ItemDataSO (abstract)
    └─ ArmorDataSO + IEquippable
```

## Поля

| Поле | Тип | Описание |
|---|---|---|
| `DisplayName` | string | Имя в инвентаре (из ItemDataSO) |
| `Icon` | Sprite | Иконка (из ItemDataSO) |
| `Slot` | EquipmentSlot | Head / Torso / Gloves / Boots |
| `PhysicalDefense` | float | Вычитается из физического урона |
| `MagicDefense` | float | Вычитается из магического урона |

> Слот **Weapon** для брони не используется — только для оружия.

## Как работает защита

[[Classes/Combat/Controllers/EquipmentController\|EquipmentController]].`GetTotalDefense(DamageType)` суммирует защиту всех надетых брон:

```csharp
// DamageType.Physical → сумма PhysicalDefense по всем слотам
// DamageType.Magic    → сумма MagicDefense по всем слотам
```

Вычитается из урона в [[HealthComponent]].`ApplyDamage()`:
```
finalDamage = max(0, incomingDamage - totalDefense)
```

## Нет визуальной смены модели

Броня не меняет внешний вид персонажа по умолчанию. Подробнее → [[HowTo/AddNewArmor]].

## Связанные файлы

- [[HowTo/AddNewArmor]] — пошаговая инструкция
- [[Classes/Combat/Controllers/EquipmentController\|EquipmentController]] — хранит надетые предметы
- [[HealthComponent]] — применяет защиту к урону
