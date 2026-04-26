# Как изменить комбо (добавить/заменить удар)

## Изменить урон/тайминг существующего удара

Открой нужный `AttackActionSO` (Assets → найди через Project окно) и поменяй цифры в инспекторе. Код менять не нужно.

| Параметр | Где |
|---|---|
| Урон удара | `DamageMultiplier` в `AttackActionSO` |
| Скорость анимации (через фазы) | `WindupDuration`, `HitDuration`, `RecoveryDuration` |
| Стамина за удар | `StaminaCostOverride` (0 = брать из WeaponDataSO) |

---

## Добавить 5-й удар в комбо

1. Создай новый `AttackActionSO`.
2. Открой `WeaponDataSO` нужного оружия.
3. В поле `Combo` добавь новый элемент и перетащи новый SO.

> `ComboController` автоматически поддерживает любое количество ударов. При достижении конца списка комбо сбрасывается.

---

## Изменить окно между ударами

Поле `Combo Window` в `WeaponDataSO` (секунды). Если игрок не нажал атаку за это время — комбо сбрасывается на первый удар.

---

## Изменить базовую формулу множителя комбо

Файл: `Combat/Runtime/DamageCalculator.cs`, константа:
```csharp
private const float ComboStepMultiplier = 1.15f;
```
Меняй прямо здесь. Это влияет на все оружия.

---

## Что происходит технически

`ComboController.GetNextAction(weapon)`:
- Проверяет `Time.time - _lastAttackTime > weapon.ComboWindow` → сброс.
- Возвращает `weapon.Combo[_nextIndex]`.
- `_nextIndex` инкрементируется, при переполнении → 0.

`DamageCalculator.Compute(...)`:
- `comboIndex` берётся из `ComboController.CurrentIndex` в момент `AttackState.Enter()`.
- `1.15^comboIndex` применяется как множитель.
