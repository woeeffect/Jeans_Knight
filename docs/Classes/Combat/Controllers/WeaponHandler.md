# WeaponHandler

**Путь**: `Assets/Scripts/Combat/Controllers/WeaponHandler.cs`
**Namespace**: `Combat.Controllers`

## Ответственность

Держит текущее оружие (данные + экземпляр в руке). Активирует хитбокс или спавнит снаряд в фазе Hit.

## Что изменить здесь

| Задача                            | Что менять                      |
| --------------------------------- | ------------------------------- |
| Изменить точку крепления оружия   | Поле `weaponSocket` в Inspector |
| Изменить точку спавна снаряда     | Поле `muzzleSocket` в Inspector |
| Изменить логику смены оружия      | Метод `Equip(WeaponDataSO)`     |
| Добавить эффекты при смене оружия | В метод `Equip()`               |

## Методы

| Метод | Кто вызывает | Что делает |
|---|---|---|
| `HandleHitPhase(damage)` | `AttackState.EnterPhase(Hit)` | Активирует хитбокс (melee) или спавнит снаряд (ranged) |
| `HandleHitEnd()` | `AttackState.Exit()` / `EnterPhase(Recovery)` | Деактивирует хитбокс |
| `Equip(weapon)` | `OnEquipmentChanged` | Уничтожает старый префаб, создаёт новый |
| `SpawnProjectile(damage)` | `HandleHitPhase` (для Ranged) | Создаёт снаряд в точке muzzleSocket |

## Смена оружия через EventBus

`WeaponHandler` реализует `IEquipmentChangedHandler`:
```
EquipmentController.Equip(item)
    └─ EventBus: IEquipmentChangedHandler
        └─ WeaponHandler.OnEquipmentChanged(slot, old, new)
            └─ if slot == Weapon: Equip(newWeaponDataSO)
```

## Связанные файлы

- [[WeaponHitbox]] — хранится на инстанцированном HeldPrefab
- [[Projectile]] — спавнится из muzzleSocket при дальних атаках
- [[AttackState]] — вызывает HandleHitPhase / HandleHitEnd
- [[DataObjects/WeaponDataSO]] — данные текущего оружия
- [[EquipmentController]] — посылает событие IEquipmentChangedHandler

## Сокеты в Inspector

- **weaponSocket**: Transform на кости руки в rig-е персонажа. Новый префаб оружия становится дочерним к нему.
- **muzzleSocket**: Transform на стволе/конце оружия. Используется как точка спавна снарядов.
- **initialWeapon**: `WeaponDataSO` для автоматической экипировки на старте (Start).
