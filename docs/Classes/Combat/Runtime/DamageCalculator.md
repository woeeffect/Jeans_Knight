# DamageCalculator

**Путь**: `Assets/Scripts/Combat/Runtime/DamageCalculator.cs`
**Namespace**: `Combat.Runtime`
**Тип**: `static class` — без экземпляра, без компонента

## Ответственность

Единственная точка, где вычисляется итоговый урон. Принимает данные из ScriptableObject-ов и возвращает готовый [[Classes/Combat/Runtime/HealthComponent\|DamageInfo]] struct.

## Сигнатура

```csharp
public static DamageInfo Compute(
    WeaponDataSO weapon,
    AttackActionSO action,
    CharacterStatsSO attackerStats,
    int comboIndex,
    GameObject attacker)
```

Вызывается из [[AttackState]].`Enter()`.

## Формула

```
stat = attackerStats.Strength     (если weapon.DamageType == Physical)
     = attackerStats.MagicPower   (если weapon.DamageType == Magic)

damage = weapon.BaseDamage
       × (1 + stat × attackerStats.StatScaling)   ← бонус от характеристик
       × 1.15 ^ comboIndex                         ← бонус комбо
       × action.DamageMultiplier                   ← множитель конкретного удара
```

Константа `ComboStepMultiplier = 1.15f` объявлена прямо в файле.

## Что изменить здесь

| Задача | Где |
|---|---|
| Базовый множитель комбо | Константа `ComboStepMultiplier` в этом файле |
| Формулу масштабирования характеристик | Строка `statMult` в методе |
| Добавить критический удар | После строки `float amount = ...` |
| Добавить тип урона «Огонь» | Расширить enum `DamageType.cs` + ветку в формуле |

## Связанные файлы

- [[DataObjects/CharacterStatsSO]] — откуда берётся `stat` и `statScaling`
- [[DataObjects/WeaponDataSO]] — откуда берётся `baseDamage` и `DamageType`
- [[AttackState]] — кто вызывает `Compute()`
- [[BlockController]] — кто применяет результат (через [[HealthComponent]])
