# AnimatorParams

**Путь**: `Assets/Scripts/Character/AnimatorParams.cs`
**Namespace**: `Character`
**Тип**: `static class` — просто константы

## Ответственность

Хранит pre-calculated хеши параметров Animator Controller. Использование `Animator.StringToHash()` один раз вместо строк в каждом вызове — экономит производительность.

## Параметры

| Константа        | Тип в Animator | Кто использует                                                                    |
| ---------------- | -------------- | --------------------------------------------------------------------------------- |
| `IsMoving`       | Bool           | [[Classes/StateMachine/States/IdleWalkRunStates\|IdleState, WalkState]]           |
| `IsRunning`      | Bool           | [[Classes/StateMachine/States/IdleWalkRunStates\|RunState, WalkState]]            |
| `Speed`          | Float          | [[Classes/StateMachine/States/IdleWalkRunStates\|IdleState, WalkState, RunState]] |
| `Attack`         | Trigger        | (устарел, не используется напрямую)                                               |
| `Block`          | Bool           | [[BlockState]]                                                                    |
| `Dodge`          | Trigger        | [[DodgeState]]                                                                    |
| `Hit`            | Trigger        | [[HitReactionState]]                                                              |
| `ComboIndex`     | Int            | [[AttackState]]                                                                   |
| `WeaponCategory` | Int            | [[AttackState]]                                                                   |

> Имена констант должны **точно совпадать** с именами параметров в Animator Controller. При переименовании параметра в аниматоре — обновить здесь.

## Как добавить новый параметр

```csharp
public static readonly int MyNewParam = Animator.StringToHash("MyNewParam");
```

И добавить параметр с тем же именем в Animator Controller.

## Связанные файлы

- [[AttackState]] — устанавливает `ComboIndex`, `WeaponCategory`, вызывает `SetTrigger` из `AttackActionSO`
- [[BlockState]] — `SetBool(Block, ...)`
- [[DodgeState]] — `SetTrigger(Dodge)`
- [[HitReactionState]] — `SetTrigger(Hit)`
