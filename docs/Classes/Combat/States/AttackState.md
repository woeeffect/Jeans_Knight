# AttackState

**Путь**: `Assets/Scripts/Combat/States/AttackState.cs`
**Namespace**: `Combat.States`

## Ответственность

Управляет одним ударом: выбирает `AttackActionSO`, вычисляет урон, управляет фазами, активирует хитбокс/снаряд.

## Фазы

```
Windup → Hit → Recovery → Done → (IdleState)
```

Каждая фаза имеет:
- **fallback-таймер** из `AttackActionSO` (работает всегда)
- **Animation Event** с `HeroController` (приоритет, если настроен)

Флаг `_animEventReceivedForPhase` — если Event пришёл, таймер игнорируется.

## Что изменить здесь

| Задача                                    | Что менять                              |
| ----------------------------------------- | --------------------------------------- |
| Изменить тайминги фаз                     | `AttackActionSO` в Inspector, не код    |
| Изменить условие отмены атаки             | `HeroController.CanStartCombatAction()` |
| Добавить эффект при ударе (VFX/звук)      | В `EnterPhase(AttackPhase.Hit)`         |
| Изменить поведение при окончании Recovery | В `AdvancePhase()` → `case Recovery:`   |

## Ключевые методы

| Метод | Когда вызывается |
|---|---|
| `Enter()` | При входе в AttackState — инициализация |
| `NotifyWindupEnd()` | Relay из [[HeroController]].AnimEvent_WindupEnd |
| `NotifyHitStart()` | Relay из [[HeroController]].AnimEvent_HitStart |
| `NotifyHitEnd()` | Relay из [[HeroController]].AnimEvent_HitEnd |
| `NotifyRecoveryEnd()` | Relay из [[HeroController]].AnimEvent_RecoveryEnd |
| `IsInCancellablePhase` | `true` если Windup или Recovery |

## Связанные файлы

- [[ComboController]] — выбирает следующий AttackActionSO
- [[WeaponHandler]] — активирует хитбокс / спавнит снаряд
- [[DamageCalculator]] — вычисляет DamageInfo в Enter()
- [[DataObjects/WeaponDataSO]] — данные оружия
- [[AnimatorParams]] — константы параметров аниматора
