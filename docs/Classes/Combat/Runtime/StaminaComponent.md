# StaminaComponent

**Путь**: `Assets/Scripts/Combat/Runtime/StaminaComponent.cs`
**Namespace**: `Combat.Runtime`
**На объекте**: персонаж игрока (рядом с [[HealthComponent]])

## Ответственность

Хранит текущую стамину, восстанавливает её каждый кадр (с задержкой после трат), предоставляет два метода списания.

## Параметры (Inspector)

| Поле | Описание | По умолчанию |
|---|---|---|
| `stats` | [[DataObjects/CharacterStatsSO\|CharacterStatsSO]] — берёт MaxStamina и StaminaRegenPerSec | — |
| `regenDelay` | Пауза (сек) перед возобновлением регена после трат | 0.5 |

## Методы

| Метод | Возвращает | Описание |
|---|---|---|
| `TryConsume(float amount)` | `bool` | Списывает всю сумму или ничего. `false` = стамины не хватает |
| `ConsumeUpTo(float amount)` | `float` consumed | Списывает сколько есть, возвращает фактически списанное |

### Когда что использовать

- **`TryConsume`** — когда действие либо происходит полностью, либо не происходит (нет стамины → нельзя). Пример: рывок должен быть запрещён.
- **`ConsumeUpTo`** — когда действие происходит всегда, но стамина может иссякнуть в процессе. Пример: [[BlockController]] при блоке — блок пробивается, если стамина кончается.

## Свойства

| Свойство | Тип | Описание |
|---|---|---|
| `Current` | float | Текущая стамина |
| `Max` | float | Максимальная стамина |
| `HasAny` | bool | `Current > 0` |

## Механика регенерации

Каждый кадр в `Update()`:
```
если Time.time < _regenSuppressedUntil → пропустить
иначе → Current += StaminaRegenPerSec × deltaTime (не выше Max)
```

После любого `TryConsume` / `ConsumeUpTo`: `_regenSuppressedUntil = Time.time + regenDelay`.

## Что изменить здесь

| Задача | Где |
|---|---|
| Скорость регенерации | [[DataObjects/CharacterStatsSO\|CharacterStatsSO]] Inspector → `StaminaRegenPerSec` |
| Задержка до регена | `regenDelay` в Inspector |
| Максимум стамины | [[DataObjects/CharacterStatsSO\|CharacterStatsSO]] Inspector → `MaxStamina` |
