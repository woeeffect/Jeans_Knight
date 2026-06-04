# HealthComponent

**Путь**: `Assets/Scripts/Combat/Runtime/HealthComponent.cs`
**Namespace**: `Combat.Runtime`

## Ответственность

Хранит и управляет HP объекта. Точка входа для любого урона.

## Что изменить здесь

| Задача | Что менять |
|---|---|
| Изменить логику урона | Метод `ApplyDamage()` |
| Добавить резист/уязвимость | До или после строки `info.Amount - defense` |
| Добавить эффект при смерти | В блоке `if (_currentHP <= 0f)` |
| Изменить начальное HP | Поле `initialHPOverride` в Inspector (−1 = брать из CharacterStatsSO) |

## Поток ApplyDamage

```
ApplyDamage(DamageInfo info)
  ├─ IsDead или IsInvincible? → return
  ├─ BlockController.IsActive? → HandleIncomingDamage
  │   └─ Parried/Blocked → return
  ├─ Броня: info.Amount -= EquipmentController.GetTotalDefense(type)
  ├─ PreDamageFilter (если задан) — модификация снаружи
  ├─ info.Amount <= 0? → return
  ├─ HP -= amount
  ├─ EventBus: IDamageReceivedHandler
  ├─ EventBus: IHealthChangedHandler
  └─ HP == 0 → EventBus: IDeathHandler
```

## Связанные файлы

- [[BlockController]] — проверяет блок/парри до применения урона
- [[EquipmentController]] — предоставляет суммарную защиту брони
- [[DodgeState]] — устанавливает/снимает `IsInvincible`
- [[DataObjects/CharacterStatsSO]] — начальные значения HP

## IsInvincible

Флаг `public bool IsInvincible { get; set; }`.
Устанавливается в `DodgeState.Enter()` / сбрасывается в `DodgeState.Exit()`.
Можно использовать для любых временных неуязвимостей.

## PreDamageFilter

```csharp
public Func<DamageInfo, DamageInfo> PreDamageFilter;
```
Делегат для внешних модификаций урона (например, временный баф/дебаф).
Устанавливается снаружи, автоматически очищается в `OnDestroy()`.
