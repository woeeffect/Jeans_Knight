# HitReactionState

**Путь**: `Assets/Scripts/Combat/States/HitReactionState.cs`
**Namespace**: `Combat.States`
**Создаётся в**: [[HeroController]] `Awake()`

## Ответственность

Держит персонажа в состоянии «оглушён» заданное время, затем возвращает в Idle. Любой ввод в это время игнорируется (см. [[Systems/StateMachine#CanStartCombatAction() — правило прерываемости]]).

## Как попасть в это состояние

```csharp
// HeroController.Stun(float duration):
HitReactionState.EnterStun(duration);
_stateMachine.ChangeState(HitReactionState);
```

Вызывается из [[BlockController]] при успешном парировании атакующего.

## Важный нюанс: EnterStun до Enter

`EnterStun(duration)` нужно вызвать **до** `ChangeState`, потому что `ChangeState` немедленно вызывает `Enter()`. В `Enter()` есть защита: если `_exitTime` уже не задан (или в прошлом) — ставит дефолтные 0.4 сек.

## Переходы

| Выход | Условие |
|---|---|
| → `IdleState` | `Time.time >= _exitTime` (в `Update()`) |

Никаких других переходов — нельзя прервать Dodge или Attack.

## Что изменить здесь

| Задача | Где |
|---|---|
| Длительность оглушения при парри | [[BlockController]] Inspector → `parryStunDuration` |
| Дефолтная длительность (без EnterStun) | `0.4f` в строке `Enter()` |
| Анимация оглушения | Animator Controller, trigger `Hit` из [[AnimatorParams]] |
| Добавить эффект (вспышка, звук) | В `Enter()` этого файла |
