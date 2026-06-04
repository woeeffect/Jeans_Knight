# IdleState / WalkState / RunState

**Пути**:
- `Assets/Scripts/StateMachine/States/IdleState.cs`
- `Assets/Scripts/StateMachine/States/WalkState.cs`
- `Assets/Scripts/StateMachine/States/RunState.cs`

**Namespace**: `StateMachine.States`
**Создаются в**: [[HeroController]] `Awake()`

## Ответственность

Три базовых состояния движения. Простые — каждое содержит ~20 строк.

## Переходы между ними

```
IdleState.Update():
    MoveDirection.sqrMagnitude > 0.01 → WalkState

WalkState.Update():
    MoveDirection == 0 → IdleState
    IsRunning == true  → RunState

RunState.Update():
    MoveDirection == 0 → IdleState
    IsRunning == false → WalkState
```

Ввод движения (`_moveDirection`) обновляется в [[HeroController]].`OnMove()` из [[Systems/Input\|InputService]].

## Что они делают с аниматором

| Состояние | SetBool IsMoving | SetBool IsRunning | SetFloat Speed |
|---|---|---|---|
| `IdleState.Enter()` | false | false | 0 |
| `WalkState.Enter()` | true | false | walkSpeed |
| `RunState.Enter()` | true | true | runSpeed |

## Движение персонажа

`WalkState` и `RunState` каждый кадр двигают персонажа через `CharacterController.Move()`:
- Направление берётся из `_hero.MoveDirection` (камеро-относительно).
- Скорость: `WalkSpeed` или `RunSpeed` из [[HeroController]].

## Что изменить здесь

| Задача | Где |
|---|---|
| Скорость ходьбы | [[HeroController]] Inspector → `walkSpeed` |
| Скорость бега | [[HeroController]] Inspector → `runSpeed` |
| Скорость поворота | [[HeroController]] Inspector → `rotationSpeed` |
| Логика переходов | `Update()` в каждом состоянии |
