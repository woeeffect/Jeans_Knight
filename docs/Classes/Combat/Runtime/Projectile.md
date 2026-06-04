# Projectile

**Путь**: `Assets/Scripts/Combat/Runtime/Projectile.cs`
**Namespace**: `Combat.Runtime`
**Размещение**: отдельный **префаб снаряда** (указывается в [[DataObjects/WeaponDataSO\|RangedWeaponDataSO]])

## Ответственность

Физический снаряд. Летит через `Rigidbody.linearVelocity`, наносит урон при столкновении (через [[DamageApplicationHelper]]), уничтожается после первого попадания или по TTL.

## Требования к префабу

- `Rigidbody` (добавляется автоматически через `RequireComponent`).
- `Collider` — **Trigger** (если `useTrigger = true`) или обычный (если `false`).
- Меш/Visual по желанию.

## Параметр useTrigger

| Значение | Когда использовать |
|---|---|
| `true` (по умолч.) | Снаряд проходит сквозь физику, реагирует на любой `Collider` |
| `false` | Снаряд физически сталкивается (отскок/рикошет из коробки) |

## Метод Launch()

Вызывается из [[WeaponHandler]].`SpawnProjectile()` сразу после `Instantiate`:
```csharp
projectile.Launch(damage, ownerGO, direction, speed, lifetime);
```
Устанавливает скорость `Rigidbody` и запускает TTL-таймер.

## Защита от двойного урона

Флаг `_consumed = true` после первого попадания — последующие `OnTriggerEnter`/`OnCollisionEnter` игнорируются, даже если объект ещё не уничтожен.

## Поток попадания

```
OnTriggerEnter / OnCollisionEnter
  └─ _consumed = true
  └─ DamageApplicationHelper.TryApply(collider, _damage, _owner)
  └─ Destroy(gameObject)
```

## Гравитация

`Awake()` выставляет `useGravity = false` — снаряд летит прямо. Чтобы добавить дугу полёта: убери строку `_rigidbody.useGravity = false` или добавь собственный код в `Update()`.

## Что изменить здесь

| Задача | Где |
|---|---|
| Скорость снаряда | [[DataObjects/WeaponDataSO\|RangedWeaponDataSO]] Inspector → `ProjectileSpeed` |
| Время жизни | [[DataObjects/WeaponDataSO\|RangedWeaponDataSO]] Inspector → `ProjectileLifetime` |
| Точка спавна | [[WeaponHandler]] Inspector → `muzzleSocket` |
| Добавить дуговую траекторию | `Awake()` в этом файле (включить гравитацию) |
| VFX при полёте | Добавить ParticleSystem в префаб снаряда |
| VFX при попадании | Instantiate эффект в `OnTriggerEnter` перед `Destroy` |
