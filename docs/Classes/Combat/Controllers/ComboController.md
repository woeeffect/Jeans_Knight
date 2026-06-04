# ComboController

**Путь**: `Assets/Scripts/Combat/Controllers/ComboController.cs`
**Namespace**: `Combat.Controllers`

## Ответственность

Отслеживает текущую позицию в комбо и время последнего удара. Возвращает следующий `AttackActionSO`.

## Методы

| Метод | Кто вызывает | Что делает |
|---|---|---|
| `GetNextAction(weapon)` | `AttackState.Enter()` | Возвращает следующий удар, сбрасывает комбо если истёк ComboWindow |
| `Reset()` | (вручную, если нужно) | Принудительный сброс комбо на первый удар |
| `ResetIfExpired(weapon)` | (опционально) | Проверяет и сбрасывает без возврата action |

## Свойства

| Свойство | Описание |
|---|---|
| `CurrentIndex` | Индекс последнего полученного удара (0..N-1) |
| `NextIndex` | Индекс следующего удара |

## Как работает сброс комбо

```csharp
if (Time.time - _lastAttackTime > weapon.ComboWindow)
    _nextIndex = 0;
```

`ComboWindow` — поле в `WeaponDataSO` (по умолчанию 0.8 сек).

## Множитель комбо

`ComboController` **не применяет** множитель сам — он только даёт `CurrentIndex`.
Множитель `1.15^comboIndex` применяется в `DamageCalculator.Compute()`.
