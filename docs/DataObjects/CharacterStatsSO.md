# CharacterStatsSO — характеристики персонажа

**Путь**: `Assets/Scripts/Combat/Stats/CharacterStatsSO.cs`
[Create → Jeans_Knight/Stats/Character Stats]

## Поля

| Поле | Описание | По умолчанию |
|---|---|---|
| `Strength` | Статы для физического урона | 10 |
| `MagicPower` | Статы для магического урона | 10 |
| `StatScaling` | Множитель: `damage *= 1 + stat × statScaling` | 0.01 |
| `MaxHP` | Максимальное HP | 100 |
| `MaxStamina` | Максимальная стамина | 100 |
| `StaminaRegenPerSec` | Восстановление стамины в секунду | 15 |
| `BlockStaminaRatio` | Стамина на 1 единицу заблокированного урона | 1.0 |

## Формула урона

```
stat = Strength (если DamageType.Physical)
     = MagicPower (если DamageType.Magic)

finalDamage = BaseDamage × (1 + stat × StatScaling) × 1.15^comboIndex × actionMultiplier
```

## Как изменить баланс

1. Найди `CharacterStats.asset` в папке Resources или ScriptableObjects.
2. Открой в Inspector.
3. Меняй цифры — перекомпиляция не нужна.

> Каждый персонаж/враг может иметь свой экземпляр `CharacterStatsSO`.
