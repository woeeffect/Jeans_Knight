# WeaponDataSO — конфиги оружия

## Иерархия

```
ItemDataSO (abstract)
    └─ WeaponDataSO (abstract) + IEquippable
           ├─ MeleeWeaponDataSO    [Create → Jeans_Knight/Combat/Weapon/Melee Weapon]
           └─ RangedWeaponDataSO   [Create → Jeans_Knight/Combat/Weapon/Ranged Weapon]
```

## Поля WeaponDataSO (общие)

| Поле | Тип | Описание |
|---|---|---|
| `DisplayName` | string | Имя в инвентаре |
| `Icon` | Sprite | Иконка |
| `MaxStack` | int | (базовый, обычно 1) |
| `Category` | WeaponCategory | MeleePhysical / MeleeMagic / RangedPhysical / RangedMagic |
| `DamageType` | DamageType | Physical / Magic |
| `BaseDamage` | float | Базовый урон |
| `StaminaCost` | float | Стамина за удар (если нет override в Action) |
| `ComboWindow` | float | Секунды между ударами (0.8) |
| `Combo` | List<AttackActionSO> | 4 удара (или N) |
| `HeldPrefab` | GameObject | Префаб модели в руке |

## Поля MeleeWeaponDataSO

| Поле | Тип | Описание |
|---|---|---|
| `Range` | float | Для справки (размер коллайдера задаётся в префабе) |

## Поля RangedWeaponDataSO

| Поле | Тип | Описание |
|---|---|---|
| `ProjectilePrefab` | GameObject | Префаб снаряда (нужен `Projectile.cs`) |
| `ProjectileSpeed` | float | Скорость снаряда |
| `ProjectileLifetime` | float | TTL снаряда (сек) |
| `MuzzleOffset` | Vector3 | Смещение от muzzleSocket |

## AttackActionSO — один удар в комбо

[Create → Jeans_Knight/Combat/Attack Action]

| Поле | Тип | Описание |
|---|---|---|
| `AnimationTrigger` | string | Имя Trigger-параметра аниматора |
| `WindupDuration` | float | Fallback-таймер фазы замаха |
| `HitDuration` | float | Fallback-таймер активного хитбокса |
| `RecoveryDuration` | float | Fallback-таймер восстановления |
| `DamageMultiplier` | float | Множитель урона (1.0 / 1.15 / 1.3 / 1.52) |
| `StaminaCostOverride` | float | 0 = брать из WeaponDataSO |
