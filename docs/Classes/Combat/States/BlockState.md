# BlockState

**Путь**: `Assets/Scripts/Combat/States/BlockState.cs`
**Namespace**: `Combat.States`
**Создаётся в**: [[HeroController]] `Awake()`

## Ответственность

Минимальное состояние-обёртка: включает/выключает [[BlockController]] и анимацию блока. Сама логика блока (парри, поглощение урона) — в [[BlockController]].

## Жизненный цикл

| Метод | Что делает |
|---|---|
| `Enter()` | `BlockController.BeginBlock()` + `Animator.SetBool(Block, true)` |
| `Exit()` | `BlockController.EndBlock()` + `Animator.SetBool(Block, false)` |
| `Update()` | пусто — переход на Idle управляется из [[HeroController]].`ProcessBufferedInput()` |

## Переходы

```
HeroController.ProcessBufferedInput():
    Block удерживается И CanStartCombatAction() → вход в BlockState
    Block отпущен → выход в IdleState
```

## Что изменить здесь

| Задача | Где |
|---|---|
| Логика парирования | [[BlockController]] |
| Расход стамины на блок | [[BlockController]] + [[DataObjects/CharacterStatsSO\|CharacterStatsSO]].`BlockStaminaRatio` |
| Анимация блока | Animator Controller, параметр `Block` (bool) из [[AnimatorParams]] |
