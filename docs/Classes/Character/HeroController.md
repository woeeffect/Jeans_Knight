# HeroController

**Путь**: `Assets/Scripts/Character/HeroController.cs`
**Namespace**: `Character`

## Ответственность

Главный «оркестратор» персонажа игрока. Сам по себе **не содержит логики** отдельных действий — он только:
- Агрегирует все компоненты персонажа.
- Принимает сырой ввод и буферизует его.
- Решает, в какое состояние перейти.
- Предоставляет состояниям доступ к компонентам через `public` свойства.

## Что изменить здесь

| Задача | Что менять |
|---|---|
| Изменить скорость ходьбы/бега | Поля `walkSpeed`, `runSpeed` в Inspector |
| Изменить параметры уклонения | Поля `dodgeSpeed`, `dodgeDuration`, `dodgeStaminaCost`, `dodgeSpeedCurve` |
| Добавить новое состояние | Добавить свойство + инстанцировать в `Awake()` |
| Изменить правила прерывания | Методы `CanStartCombatAction()`, `CanAdvanceCombo()` |
| Добавить новый тип ввода | Реализовать новый `IInput*Handler` + поле-буфер + обработку в `ProcessBufferedInput()` |

## Компоненты (GetComponent в Awake)

| Свойство | Тип | Роль |
|---|---|---|
| `CharacterController` | Unity | Движение и физика |
| `Animator` | Unity | Анимации |
| `Health` | [[HealthComponent]] | HP персонажа |
| `Stamina` | [[StaminaComponent]] | Стамина |
| `ComboController` | [[ComboController]] | Цепочки ударов |
| `WeaponHandler` | [[WeaponHandler]] | Оружие в руке |
| `EquipmentController` | [[EquipmentController]] | Надетые предметы |

## Состояния (инстанцируются в Awake)

[[IdleWalkRunStates\|IdleState]], [[IdleWalkRunStates\|WalkState]], [[IdleWalkRunStates\|RunState]], [[AttackState]], [[BlockState]], [[DodgeState]], [[HitReactionState]]

## Animation Event relay-методы

Animation Events в клипах вызывают эти методы на `HeroController` (не на состояниях напрямую):
```
AnimEvent_WindupEnd()   → AttackState.NotifyWindupEnd()
AnimEvent_HitStart()    → AttackState.NotifyHitStart()
AnimEvent_HitEnd()      → AttackState.NotifyHitEnd()
AnimEvent_RecoveryEnd() → AttackState.NotifyRecoveryEnd()
```

Все имена параметров аниматора — в [[AnimatorParams]].

## Метод Stun()

```csharp
public void Stun(float duration)
```
Вызывается из [[BlockController]] когда игрок успешно парирует. Переводит в [[HitReactionState]] на указанное время.
