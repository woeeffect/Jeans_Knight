# Как добавить новое оружие

## Шаг 1: Создать AttackActionSO для каждого удара в комбо

В Project window: **ПКМ → Create → Jeans_Knight / Combat / Attack Action**

Настроить поля:
| Поле | Описание | Пример |
|---|---|---|
| Animation Trigger | Строка-триггер аниматора | `"SwordAttack1"` |
| Windup Duration | Сек. фазы замаха (fallback) | `0.2` |
| Hit Duration | Сек. активного хитбокса (fallback) | `0.12` |
| Recovery Duration | Сек. восстановления (fallback) | `0.3` |
| Damage Multiplier | Множитель урона | `1.0`, `1.15`, `1.3`, `1.52` |
| Stamina Cost Override | 0 = брать из WeaponDataSO | `0` |

> Создай 4 штуки — по одному на каждый удар в комбо.

---

## Шаг 2: Создать WeaponDataSO

**Ближнее оружие**: **ПКМ → Create → Jeans_Knight / Combat / Weapon / Melee Weapon**
**Дальнее оружие**: **ПКМ → Create → Jeans_Knight / Combat / Weapon / Ranged Weapon**

Общие поля (`WeaponDataSO`):
| Поле | Описание |
|---|---|
| Display Name | Название в инвентаре |
| Icon | Спрайт-иконка |
| Category | MeleePhysical / MeleeMagic / RangedPhysical / RangedMagic |
| Damage Type | Physical / Magic |
| Base Damage | Базовый урон |
| Stamina Cost | Стамина за удар (если нет override в Action) |
| Combo Window | Время окна между ударами (сек, обычно 0.8) |
| Combo | Список из 4 AttackActionSO |
| Held Prefab | Префаб модели в руке |

Дополнительно для **MeleeWeaponDataSO**:
| Поле | Описание |
|---|---|
| Range | Радиус хитбокса (используется в размере коллайдера) |

Дополнительно для **RangedWeaponDataSO**:
| Поле | Описание |
|---|---|
| Projectile Prefab | Префаб снаряда (нужен Projectile.cs + Rigidbody) |
| Projectile Speed | Скорость снаряда |
| Projectile Lifetime | TTL снаряда (сек) |
| Muzzle Offset | Смещение точки спавна от muzzleSocket |

---

## Шаг 3: Создать префаб оружия в руке (Held Prefab)

Для **ближнего** оружия:
1. Создай префаб с мешем.
2. Добавь дочерний GameObject с `WeaponHitbox.cs` + `Collider (trigger)`.
3. Настрой размер коллайдера.

Для **дальнего** оружия:
1. Создай префаб (просто меш, хитбокса не нужно).
2. Снаряд — отдельный префаб с `Projectile.cs` + `Rigidbody` + `Collider`.

---

## Шаг 4: Добавить предмет в инвентарь

`InventoryService.PlayerInventory.TryAdd(new ItemInstance(myWeaponSO, 1))`

Или вручную в старт-сцене через код / инспектор `InventoryPanel`.

---

## Шаг 5: Добавить триггеры в аниматор

Для каждого значения `AnimationTrigger` из `AttackActionSO`:
1. Открой Animator Controller.
2. Добавь параметр Trigger с тем же именем.
3. Создай переход в состояние анимации удара.
4. Добавь Animation Events на клип: `AnimEvent_WindupEnd`, `AnimEvent_HitStart`, `AnimEvent_HitEnd`, `AnimEvent_RecoveryEnd`.
   - Target: объект с `HeroController`.

> Без Animation Events будут работать fallback-таймеры — функционально правильно, но менее точно по времени.
