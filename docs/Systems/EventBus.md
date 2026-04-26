# EventBus — шина событий

## Файлы

| Файл | Роль |
|---|---|
| `EventBus/EventBusSystem/EventBus.cs` | Статический класс, точка входа |
| `EventBus/EventBusSystem/IGlobalSubscriber.cs` | Маркерный интерфейс (пустой) |
| `EventBus/EventBusSystem/SubscribersList.cs` | Внутренний список подписчиков |

## Как работает

**Подписка** (в MonoBehaviour):
```csharp
private void OnEnable()  => EventBus.Subscribe(this);
private void OnDisable() => EventBus.Unsubscribe(this);
```
Класс должен реализовывать хотя бы один `I*Handler` интерфейс.

**Отправка события** (из любого места):
```csharp
EventBus.RaiseEvent<IHealthChangedHandler>(h =>
    h.OnHealthChanged(gameObject, _currentHP, _maxHP));
```

**Получение события** (реализовать интерфейс):
```csharp
public class MyUI : MonoBehaviour, IHealthChangedHandler
{
    private void OnEnable()  => EventBus.Subscribe(this);
    private void OnDisable() => EventBus.Unsubscribe(this);

    public void OnHealthChanged(GameObject who, float current, float max)
    {
        // обновить UI
    }
}
```

---

## Все интерфейсы-события проекта

### Ввод (Input/)
| Интерфейс | Метод | Кто отправляет | Кто получает |
|---|---|---|---|
| `IInputMoveHandler` | `OnMove(Vector2)` | InputService | HeroController |
| `IInputRunHandler` | `OnRun(bool)` | InputService | HeroController |
| `IInputAttackHandler` | `OnAttack()` | InputService | HeroController |
| `IInputDodgeHandler` | `OnDodge()` | InputService | HeroController |
| `IInputBlockHandler` | `OnBlock(bool held)` | InputService | HeroController |
| `IInputInventoryHandler` | `OnOpenInventory()` | InputService | InventoryPanel |

### Боевые (Combat/Events/)
| Интерфейс | Метод | Кто отправляет | Кто получает |
|---|---|---|---|
| `IDamageReceivedHandler` | `OnDamageReceived(go, info, hp)` | HealthComponent | DummyTarget, UI |
| `IHealthChangedHandler` | `OnHealthChanged(go, current, max)` | HealthComponent | UI хп-бар |
| `IDeathHandler` | `OnDeath(go)` | HealthComponent | DummyTarget, игровая логика |
| `IBlockHandler` | `OnBlocked(go, info, stamina, broken)` | BlockController | UI, эффекты |
| `IParrySuccessHandler` | `OnParry(defender, attacker, info)` | BlockController | UI, звук, эффекты |
| `IEquipmentChangedHandler` | `OnEquipmentChanged(slot, old, new)` | EquipmentController | WeaponHandler, UI |

---

## Правило именования

Все интерфейсы событий:
- Наследуют `IGlobalSubscriber`
- Называются `I<Существительное>Handler`
- Метод называется `On<Существительное>(...)`
