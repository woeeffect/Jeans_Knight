# DamageApplicationHelper

**Путь**: `Assets/Scripts/Combat/Runtime/DamageApplicationHelper.cs`
**Namespace**: `Combat.Runtime`
**Тип**: `static class` — без экземпляра

## Ответственность

Вспомогательный класс-«переходник»: получает `Collider` от физики, находит цель (с учётом `Rigidbody`), проверяет что это не сам владелец, вызывает [[HealthComponent\|IDamageable.ApplyDamage()]].

Используется в двух местах чтобы не дублировать код:
- [[WeaponHitbox]].`OnTriggerEnter()`
- [[Projectile]].`OnTriggerEnter()` / `OnCollisionEnter()`

## Метод TryApply

```csharp
public static void TryApply(Collider other, DamageInfo damage, GameObject ignoreOwner)
```

| Шаг | Что проверяется |
|---|---|
| 1 | `target = other.attachedRigidbody?.gameObject ?? other.gameObject` |
| 2 | `target.transform.IsChildOf(ignoreOwner)` → пропуск (не бить себя) |
| 3 | `target.TryGetComponent<IDamageable>()` → нет компонента → пропуск |
| 4 | `damageable.IsDead` → уже мёртв → пропуск |
| 5 | `damageable.ApplyDamage(damage)` |

## Зачем IsChildOf а не ReferenceEquals

Когда у персонажа `Rigidbody` на корневом объекте, а `Collider` на дочернем, `other.attachedRigidbody.gameObject` будет корневой объект. `IsChildOf` гарантирует, что и дочерние коллайдеры владельца не считаются целями.
