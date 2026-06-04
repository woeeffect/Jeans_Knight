# Система ввода

## Файлы

| Файл                                                  | Роль                                                     |
| ----------------------------------------------------- | -------------------------------------------------------- |
| `Assets/Scripts/Input/InputService.cs`                | Единственный класс, который читает нажатия               |
| `Assets/InputActions/PlayerInputActions.inputactions` | Исходник бинд-таблицы (редактируется в Unity)            |
| `Assets/InputSystem_Actions.cs`                       | Авто-сгенерированный C# класс (не редактировать вручную) |
| `Assets/Scripts/Input/IInput*Handler.cs`              | Интерфейсы-«подписки» на события ввода                   |

## Как это работает

```
Клавиша/кнопка
    │
    ▼
InputSystem_Actions (Unity Input System)
    │  OnDodgePerformed, OnAttackPerformed, ...
    ▼
InputService
    │  EventBus.RaiseEvent<IInputAttackHandler>(...)
    ▼
EventBus
    │
    ▼
HeroController (реализует нужные интерфейсы)
```

`InputService` — это **не MonoBehaviour**. Он реализует `IInitializable` и `IDisposable` — Zenject вызывает `Initialize()` при старте и `Dispose()` при остановке.

## Таблица биндингов

| Действие | Клавиша | Геймпад | Интерфейс |
|---|---|---|---|
| Move | WASD | Left Stick | `IInputMoveHandler` |
| Run | Shift | Right Stick Press | `IInputRunHandler` |
| Attack | LMB | West Button | `IInputAttackHandler` |
| Dodge | Space | South Button | `IInputDodgeHandler` |
| Block | RMB | Left Trigger | `IInputBlockHandler` |
| OpenInventory | I | Start | `IInputInventoryHandler` |

> **Важно**: Block и OpenInventory добавлены **программно** в `InputService.Initialize()` через `new InputAction(...)`, а не через `.inputactions` файл.

## Как добавить новое управление

→ [[HowTo/AddNewInputAction]]

## Интерфейс IInputBlockHandler

Особенность: метод `OnBlock(bool held)` — `true` = нажали, `false` = отпустили. Это позволяет отслеживать **удержание** кнопки.

Все остальные Input-интерфейсы — одиночные события (нажатие).
