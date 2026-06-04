# WeaponHitbox

**Путь**: `Assets/Scripts/Combat/Runtime/WeaponHitbox.cs`
**Namespace**: `Combat.Runtime`
**Размещение**: дочерний GameObject на **префабе оружия** (не на персонаже)

## Ответственность

Trigger-коллайдер ближнего оружия. Активируется на время фазы Hit, регистрирует попадания, применяет урон через [[DamageApplicationHelper]]. Исключает повторное попадание по одной и той же цели за один удар.

## Настройка в префабе

1. Создай дочерний пустой GameObject внутри `HeldPrefab` оружия.
2. Добавь `WeaponHitbox.cs`.
3. Добавь `Collider` (Sphere/Box/Capsule) и пометь **Is Trigger = true**.
4. Настрой размер коллайдера под радиус атаки.

> `WeaponHitbox.Awake()` сам выставляет `isTrigger = true` и отключает коллайдер.

## Методы

| Метод | Кто вызывает | Что делает |
|---|---|---|
| `SetOwner(GameObject)` | [[WeaponHandler]].`Equip()` | Запоминает владельца — исключается из попаданий |
| `Activate(DamageInfo)` | [[WeaponHandler]].`HandleHitPhase()` | Включает коллайдер, очищает список поражённых |
| `Deactivate()` | [[WeaponHandler]].`HandleHitEnd()` | Выключает коллайдер, очищает список |

## Защита от мульти-хита

`HashSet<GameObject> _alreadyHit` — за одну активацию каждый объект получает урон **не более одного раза**. Список очищается при `Activate()` и `Deactivate()`.

## Поток попадания

```
OnTriggerEnter(Collider other)
  └─ target = other.attachedRigidbody?.gameObject ?? other.gameObject
  └─ _alreadyHit.Add(target) — если уже есть → пропуск
  └─ DamageApplicationHelper.TryApply(other, damage, _ownerRoot)
```

## Что изменить здесь

| Задача | Где |
|---|---|
| Размер зоны поражения | Коллайдер в префабе оружия |
| Тип коллайдера | Заменить Collider-компонент в префабе |
| Логика применения урона | [[DamageApplicationHelper]] |
