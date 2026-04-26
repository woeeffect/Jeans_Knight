# DodgeState

**Путь**: `Assets/Scripts/Combat/States/DodgeState.cs`
**Namespace**: `Combat.States`
**Создаётся в**: [[HeroController]] `Awake()`

## Ответственность

Управляет рывком: определяет направление, двигает персонажа по кривой, включает iframes (неуязвимость) на всё время рывка.

## Параметры (через HeroController Inspector)

| Поле HeroController | Описание | По умолчанию |
|---|---|---|
| `dodgeSpeed` | Максимальная скорость рывка | 9 |
| `dodgeDuration` | Длительность рывка (сек) | 0.28 |
| `dodgeStaminaCost` | Стамина за рывок | 20 |
| `dodgeSpeedCurve` | AnimationCurve: скорость по времени (0→1) | EaseInOut(0,1 → 1,0) |

## Логика направления

```
Если есть ввод движения (WASD) → камеро-относительное направление
Если ввода нет → вперёд по transform персонажа
```

## iframes

В `Enter()`: `HealthComponent.IsInvincible = true`
В `Exit()`: `HealthComponent.IsInvincible = false`

Весь урон в [[HealthComponent]].`ApplyDamage()` проверяет этот флаг и отклоняется.

## Переход на Idle

Автоматически в `Update()` когда `_timer >= dodgeDuration`.

## Что изменить здесь

| Задача | Где |
|---|---|
| Дистанция/скорость рывка | `dodgeSpeed`, `dodgeDuration` в [[HeroController]] Inspector |
| Форма кривой скорости | `dodgeSpeedCurve` в [[HeroController]] Inspector |
| Стоимость рывка | `dodgeStaminaCost` в [[HeroController]] Inspector |
| Анимация рывка | Animator Controller, trigger `Dodge` из [[AnimatorParams]] |
| Добавить следовой эффект (VFX) | В `Enter()` этого файла |
