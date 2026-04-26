# BlockController

**Путь**: `Assets/Scripts/Combat/Controllers/BlockController.cs`
**Namespace**: `Combat.Controllers`

## Ответственность

Управляет состоянием блока: записывает время начала, проверяет парри-окно, обрабатывает входящий урон при активном блоке.

## Параметры (Inspector)

| Поле | Описание | По умолчанию |
|---|---|---|
| `parryWindowStart` | Начало парри-окна (сек после нажатия) | 0.1 |
| `parryWindowEnd` | Конец парри-окна (сек после нажатия) | 0.25 |
| `frontArc` | Угол (градусы), в котором блок работает | 200° |
| `parryStunDuration` | Длительность оглушения атакующего при парри | 1.25 сек |

## Логика HandleIncomingDamage

```
Атакующий в frontArc?
  └─ НЕТ → NotBlocking (урон проходит)
  └─ ДА:
      elapsed = Time.time - _blockStartTime
      0.1 ≤ elapsed ≤ 0.25 → Parried (оглушить, урон = 0)
      стамина >= damage     → Blocked (стамина -= damage, урон = 0)
      стамина < damage      → BlockBroken (часть урона проходит)
```

## Что изменить здесь

| Задача | Что менять |
|---|---|
| Изменить окно парирования | Поля `parryWindowStart/End` в Inspector |
| Изменить угол блока | Поле `frontArc` |
| Добавить reflect damage при парри | В блоке `if (IsInParryWindow())` — вызвать ApplyDamage на атакующем |
| Изменить расход стамины на блок | Логика в `else` блоке — сейчас `staminaNeeded = info.Amount` |
