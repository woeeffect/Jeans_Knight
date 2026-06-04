# Как добавить новую кнопку управления

## Быстрый путь (программный, как Block и OpenInventory)

Этот путь не требует регенерации `InputSystem_Actions.cs`.

### 1. Создать интерфейс события

```csharp
// Assets/Scripts/Input/IInputMyActionHandler.cs
using EventBusSystem;

namespace Input
{
    public interface IInputMyActionHandler : IGlobalSubscriber
    {
        void OnMyAction();
    }
}
```

### 2. Добавить InputAction в InputService

```csharp
// InputService.cs — поле
private InputAction _myAction;

// InputService.Initialize()
_myAction = new InputAction("MyAction", InputActionType.Button);
_myAction.AddBinding("<Keyboard>/someKey").WithGroup(KeyboardMouseControlScheme);
_myAction.AddBinding("<Gamepad>/someButton").WithGroup(GamepadControlScheme);
_myAction.performed += OnMyActionPerformed;
_myAction.Enable();

// InputService.Dispose()
if (_myAction != null)
{
    _myAction.performed -= OnMyActionPerformed;
    _myAction.Disable();
    _myAction.Dispose();
    _myAction = null;
}

// Обработчик
private void OnMyActionPerformed(InputAction.CallbackContext ctx)
{
    UpdateControlScheme(ctx.control?.device);
    EventBus.RaiseEvent<IInputMyActionHandler>(h => h.OnMyAction());
}
```

### 3. Подписаться в нужном MonoBehaviour

```csharp
public class MyComponent : MonoBehaviour, IInputMyActionHandler
{
    private void OnEnable()  => EventBus.Subscribe(this);
    private void OnDisable() => EventBus.Unsubscribe(this);

    public void OnMyAction()
    {
        // логика
    }
}
```

---

## Полный путь (через .inputactions файл)

Используй этот путь если хочешь бинд в UI Input System Editor.

1. Открой `Assets/InputActions/PlayerInputActions.inputactions` в Unity.
2. Добавь новый Action в группу Player.
3. Назначь биндинги.
4. Нажми **Generate C# Class** (кнопка в Inspector файла).
5. В `InputService` добавь подписку:
   ```csharp
   _playerInput.Player.MyAction.performed += OnMyActionPerformed;
   // + отписка в Dispose()
   ```

> **Внимание**: после регенерации проверь, что Jump не вернулся (мы переименовали его в Dodge).
