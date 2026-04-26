# Как подписаться на игровое событие (EventBus)

## Подписаться на существующее событие

Пример: хочу обновлять полоску HP в UI при изменении здоровья.

```csharp
using Combat.Events;
using EventBusSystem;
using UnityEngine;

public class HealthBar : MonoBehaviour, IHealthChangedHandler
{
    private void OnEnable()  => EventBus.Subscribe(this);
    private void OnDisable() => EventBus.Unsubscribe(this);

    public void OnHealthChanged(GameObject who, float current, float max)
    {
        // обновить слайдер
        slider.value = current / max;
    }
}
```

**Правило**: всегда пара `Subscribe` в `OnEnable` + `Unsubscribe` в `OnDisable`.

---

## Создать новое событие

### 1. Создать интерфейс

```csharp
// Например: Combat/Events/IMyGameEventHandler.cs
using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IMyGameEventHandler : IGlobalSubscriber
    {
        void OnMyGameEvent(GameObject source, float value);
    }
}
```

### 2. Отправить событие из кода

```csharp
EventBus.RaiseEvent<IMyGameEventHandler>(h =>
    h.OnMyGameEvent(gameObject, someValue));
```

### 3. Получить событие

Как в примере выше — реализовать интерфейс + Subscribe/Unsubscribe.

---

## Все текущие события

→ [[Systems/EventBus]] — полная таблица интерфейсов

---

## Частые ошибки

| Проблема | Причина |
|---|---|
| Событие не приходит | Забыл `EventBus.Subscribe(this)` в `OnEnable` |
| Событие приходит после Destroy | Не вызвал `EventBus.Unsubscribe(this)` в `OnDisable` |
| Компилятор ругается | Не наследовал `IGlobalSubscriber` в интерфейсе |
