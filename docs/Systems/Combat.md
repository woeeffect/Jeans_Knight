# Боевая система

## Поток одного удара (LMB → урон)

```
1. Игрок нажимает LMB
   └─ InputService → EventBus → HeroController.OnAttack()
      └─ _attackRequested = true

2. HeroController.ProcessBufferedInput()
   └─ CanAdvanceCombo() == true?
      └─ _stateMachine.ChangeState(AttackState)

3. AttackState.Enter()
   ├─ ComboController.GetNextAction(weapon)  → выбирает AttackActionSO
   ├─ DamageCalculator.Compute(...)          → создаёт DamageInfo
   ├─ Аниматор: SetTrigger(action.AnimationTrigger)
   └─ EnterPhase(Windup)  → запускает fallback-таймер

4. Фаза Windup (персонаж замахивается)
   └─ Animation Event "AnimEvent_WindupEnd" ИЛИ таймер иссяк
      └─ EnterPhase(Hit)

5. Фаза Hit (активен хитбокс)
   ├─ MeleeWeapon → WeaponHandler.HandleHitPhase(damage)
   │                └─ WeaponHitbox.Activate(damage)
   └─ RangedWeapon → WeaponHandler.HandleHitPhase(damage)
                     └─ WeaponHandler.SpawnProjectile(damage)

6. WeaponHitbox.OnTriggerEnter / Projectile.OnCollisionEnter
   └─ DamageApplicationHelper.TryApply(other, damage, owner)
      └─ IDamageable.ApplyDamage(damage)

7. HealthComponent.ApplyDamage(info)
   ├─ IsInvincible? → выход (Dodge iframes)
   ├─ BlockController.HandleIncomingDamage(info)?
   │   ├─ Parried  → оглушить атакующего, урон = 0
   │   ├─ Blocked  → списать стамину, урон = 0
   │   └─ BlockBroken → часть урона проходит
   ├─ EquipmentController.GetTotalDefense(type) → вычесть броню
   └─ HP -= amount → EventBus: IDamageReceivedHandler, IHealthChangedHandler
      └─ HP == 0 → EventBus: IDeathHandler
```

---

## Формула урона

```
damage = weapon.BaseDamage
       × (1 + stat × statScaling)   ← stat = Strength или MagicPower
       × 1.15^comboIndex            ← комбо-множитель (+15% за каждый удар)
       × action.DamageMultiplier    ← множитель конкретного удара из SO
```

Файл: `Combat/Runtime/DamageCalculator.cs`

---

## Комбо-система

Файл: `Combat/Controllers/ComboController.cs`

| Свойство | Значение |
|---|---|
| `CurrentIndex` | Индекс последнего удара (0..3) |
| `GetNextAction(weapon)` | Возвращает следующий `AttackActionSO`, сбрасывает если истёк `ComboWindow` |
| Сброс комбо | Автоматически если между ударами прошло > `weapon.ComboWindow` (по умолчанию 0.8 сек) |

---

## Фазы атаки

| Фаза | Прерываемо? | Что происходит |
|---|---|---|
| `Windup` | ✅ Dodge / Block | Персонаж замахивается |
| `Hit` | ❌ | Хитбокс активен / снаряд выпущен |
| `Recovery` | ✅ Dodge / Block | Персонаж возвращается в исходное положение |
| `Done` | — | Переход в Idle |

Таймингом управляют **Animation Events** (приоритет) или **fallback-таймеры** из `AttackActionSO`.

Animation Events вызывают методы на `HeroController`:
- `AnimEvent_WindupEnd()`
- `AnimEvent_HitStart()`
- `AnimEvent_HitEnd()`
- `AnimEvent_RecoveryEnd()`

---

## Блок и парирование

Файл: `Combat/Controllers/BlockController.cs`

```
Игрок зажимает ПКМ
  └─ BlockState.Enter() → BlockController.BeginBlock()
     └─ _blockStartTime = Time.time

Атака долетает до игрока
  └─ HealthComponent.ApplyDamage(info)
     └─ BlockController.HandleIncomingDamage(info)
        ├─ elapsed = Time.time - _blockStartTime
        ├─ 0.1 ≤ elapsed ≤ 0.25 → ПАРРИ (оглушить атакующего)
        ├─ elapsed > 0.25, стамины хватает → БЛОК (списать стамину)
        └─ стамины не хватает → БЛОК ПРОБИТ (часть урона проходит)
```

**Парри-окно**: 0.1 — 0.25 секунды после нажатия ПКМ.
Оглушение атакующего: `IStunnable.Stun(parryStunDuration)` — по умолчанию 1.25 сек.

---

## Уклонение и iframes

Файл: `Combat/States/DodgeState.cs`

- Направление: по вектору ввода (камеро-относительно) или вперёд если ввода нет.
- Скорость по кривой `dodgeSpeedCurve` (AnimationCurve в инспекторе HeroController).
- `HealthComponent.IsInvincible = true` на всё время уклонения (~0.28 сек).

---

## Манекен (тест)

Файл: `AI/DummyTarget.cs`

- Реализует `IDamageable` (есть `HealthComponent`).
- Реализует `IStunnable` — при парри от игрока меняет цвет.
- В Inspector есть кнопка **[Context Menu] Debug Attack Player** — симулирует удар по игроку для тестирования блока/парри.
