# Стейт-машина персонажа

## Файлы

| Файл | Роль |
|---|---|
| `StateMachine/StateMachine.cs` | Ядро: хранит CurrentState, вызывает Enter/Exit/Update |
| `StateMachine/IState.cs` | Интерфейс состояния: `Enter()`, `Exit()`, `Update()` |
| `StateMachine/States/IdleState.cs` | Стоит на месте |
| `StateMachine/States/WalkState.cs` | Идёт шагом |
| `StateMachine/States/RunState.cs` | Бежит |
| `Combat/States/AttackState.cs` | Атакует (фазы: Windup → Hit → Recovery) |
| `Combat/States/BlockState.cs` | Блокирует |
| `Combat/States/DodgeState.cs` | Уклоняется |
| `Combat/States/HitReactionState.cs` | Получил удар / оглушён |

## Жизненный цикл состояния

```
StateMachine.ChangeState(newState)
    │
    ├─ currentState.Exit()      ← завершить старое
    ├─ currentState = newState
    └─ currentState.Enter()     ← начать новое

StateMachine.Update()  (вызывается каждый кадр из HeroController.Update)
    └─ currentState.Update()
```

## Кто управляет переходами

Переходы инициируются из двух мест:

1. **`HeroController.ProcessBufferedInput()`** — читает буферизованный ввод и решает, в какое состояние переходить.
2. **Сами состояния** — например `AttackState` сам переходит в `IdleState` когда анимация заканчивается; `DodgeState` переходит в `IdleState` по таймеру.

## Таблица переходов

| Из → В | Условие |
|---|---|
| Любое → Dodge | Нажат Dodge + `CanStartCombatAction()` |
| Любое → Attack | Нажат Attack + `CanAdvanceCombo()` + не HitReaction + не Dodge |
| Любое → Block | Зажат Block + `CanStartCombatAction()` |
| Block → Idle | Block отпущен |
| Attack → Idle | Recovery-фаза завершена |
| Dodge → Idle | Таймер уклонения истёк |
| HitReaction → Idle | Таймер оглушения истёк |
| Idle/Walk/Run | Автоматически по `_moveDirection` и `_isRunning` |

## CanStartCombatAction() — правило прерываемости

```
HitReactionState  → нельзя прервать никем
DodgeState        → нельзя прервать никем
AttackState       → только если фаза Windup или Recovery
Всё остальное     → можно прервать
```

Поле `AttackState.IsInCancellablePhase` = `_phase == Windup || _phase == Recovery`.
