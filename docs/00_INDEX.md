# Jeans_Knight — Карта проекта

> **Как пользоваться:** открой папку `Docs/` как Vault в [Obsidian](https://obsidian.md). Все ссылки `[[...]]` кликабельны.

---

## Быстрая навигация

| Хочу понять... | Иди сюда |
|---|---|
| Общую архитектуру | [[01_Архитектура]] |
| Как работает ввод (WASD, мышь, геймпад) | [[Systems/Input]] |
| Как работает стейт-машина персонажа | [[Systems/StateMachine]] |
| Как работает боевая система | [[Systems/Combat]] |
| Как работает инвентарь и экипировка | [[Systems/Inventory]] |
| Как работает EventBus | [[Systems/EventBus]] |
| Добавить новое оружие | [[HowTo/AddNewWeapon]] |
| Добавить новую броню | [[HowTo/AddNewArmor]] |
| Добавить удар в комбо | [[HowTo/AddNewComboAttack]] |
| Добавить новое состояние персонажа | [[HowTo/AddNewState]] |
| Добавить новую кнопку управления | [[HowTo/AddNewInputAction]] |
| Подписаться на игровое событие | [[HowTo/AddNewEventBusHandler]] |

---

## Все классы

> Class-notes разложены по тематическим подпапкам внутри `Classes/`, чтобы структура в Obsidian повторяла домены проекта.

### Character & StateMachine
| Класс | Файл |
|---|---|
| [[Classes/Character/HeroController\|HeroController]] | `Character/HeroController.cs` |
| [[Classes/Character/AnimatorParams\|AnimatorParams]] | `Character/AnimatorParams.cs` |
| [[Classes/StateMachine/States/IdleWalkRunStates\|IdleState / WalkState / RunState]] | `StateMachine/States/` |

### Combat — States
| Класс | Файл |
|---|---|
| [[Classes/Combat/States/AttackState\|AttackState]] | `Combat/States/AttackState.cs` |
| [[Classes/Combat/States/BlockState\|BlockState]] | `Combat/States/BlockState.cs` |
| [[Classes/Combat/States/DodgeState\|DodgeState]] | `Combat/States/DodgeState.cs` |
| [[Classes/Combat/States/HitReactionState\|HitReactionState]] | `Combat/States/HitReactionState.cs` |

### Combat — Controllers
| Класс | Файл |
|---|---|
| [[Classes/Combat/Controllers/ComboController\|ComboController]] | `Combat/Controllers/ComboController.cs` |
| [[Classes/Combat/Controllers/BlockController\|BlockController]] | `Combat/Controllers/BlockController.cs` |
| [[Classes/Combat/Controllers/WeaponHandler\|WeaponHandler]] | `Combat/Controllers/WeaponHandler.cs` |
| [[Classes/Combat/Controllers/EquipmentController\|EquipmentController]] | `Combat/Controllers/EquipmentController.cs` |

### Combat — Runtime
| Класс | Файл |
|---|---|
| [[Classes/Combat/Runtime/HealthComponent\|HealthComponent]] | `Combat/Runtime/HealthComponent.cs` |
| [[Classes/Combat/Runtime/StaminaComponent\|StaminaComponent]] | `Combat/Runtime/StaminaComponent.cs` |
| [[Classes/Combat/Runtime/WeaponHitbox\|WeaponHitbox]] | `Combat/Runtime/WeaponHitbox.cs` |
| [[Classes/Combat/Runtime/Projectile\|Projectile]] | `Combat/Runtime/Projectile.cs` |
| [[Classes/Combat/Runtime/DamageCalculator\|DamageCalculator]] | `Combat/Runtime/DamageCalculator.cs` |
| [[Classes/Combat/Runtime/DamageApplicationHelper\|DamageApplicationHelper]] | `Combat/Runtime/DamageApplicationHelper.cs` |

### Inventory
| Класс | Файл |
|---|---|
| [[Classes/Inventory/Runtime/InventoryService\|InventoryService]] | `Inventory/Runtime/InventoryService.cs` |
| [[Classes/Inventory/UI/InventoryPanel\|InventoryPanel]] | `Inventory/UI/InventoryPanel.cs` |

### AI & Infrastructure
| Класс | Файл |
|---|---|
| [[Classes/AI/DummyTarget\|DummyTarget]] | `AI/DummyTarget.cs` |
| [[Classes/Installers/GameInstaller\|GameInstaller]] | `Installers/GameInstaller.cs` |

### ScriptableObjects (данные)
| SO | Файл |
|---|---|
| [[DataObjects/WeaponDataSO\|WeaponDataSO / MeleeWeaponDataSO / RangedWeaponDataSO]] | `Combat/Data/` |
| [[DataObjects/ArmorDataSO\|ArmorDataSO]] | `Combat/Data/ArmorDataSO.cs` |
| [[DataObjects/CharacterStatsSO\|CharacterStatsSO]] | `Combat/Stats/CharacterStatsSO.cs` |

---

## Все системы одним взглядом

```
InputService ──► EventBus ──► HeroController
                                    │
                    ┌───────────────┼────────────────┐
                    ▼               ▼                ▼
               StateMachine   HealthComponent   StaminaComponent
                    │
        ┌───────────┼───────────┬──────────┬──────────┐
        ▼           ▼           ▼          ▼          ▼
    IdleState   WalkState   AttackState BlockState DodgeState
                                │
                    ┌───────────┴───────────┐
                    ▼                       ▼
              ComboController          WeaponHandler
                                            │
                                   ┌────────┴────────┐
                                   ▼                 ▼
                              WeaponHitbox        Projectile
                                   │
                                   ▼
                              IDamageable.ApplyDamage
                                   │
                              HealthComponent
```

---

## Файловая структура Scripts/

```
Assets/Scripts/
├── AI/
│   └── DummyTarget.cs           ← тестовый манекен
├── Character/
│   ├── HeroController.cs        ← ГЛАВНЫЙ класс персонажа
│   └── AnimatorParams.cs        ← константы хешей аниматора
├── Combat/
│   ├── Controllers/             ← компоненты боевой логики
│   ├── Data/                    ← ScriptableObject-конфиги
│   ├── Events/                  ← интерфейсы EventBus
│   ├── Runtime/                 ← рантайм: HP, стамина, хитбоксы
│   ├── States/                  ← боевые состояния
│   └── Stats/                   ← DamageInfo + CharacterStatsSO
├── EventBus/                    ← ядро шины событий
├── Input/                       ← InputService + интерфейсы ввода
├── Installers/                  ← Zenject GameInstaller
├── Inventory/                   ← данные, рантайм, UI инвентаря
└── StateMachine/                ← ядро стейт-машины + Idle/Walk/Run
```
