using System;
using System.Collections.Generic;
using EventBusSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Input
{
    public class InputService : IInitializable, IDisposable
    {
        private const string KeyboardMouseControlScheme = "Keyboard&Mouse";
        private const string GamepadControlScheme = "Gamepad";

        private InputSystem_Actions _playerInput;
        private string _currentControlScheme;

        public void Initialize()
        {
            _playerInput = new InputSystem_Actions();
            InputSystem.onDeviceChange += OnDeviceChange;
            ApplyInitialControlScheme();
            _playerInput.Player.Enable();

            _playerInput.Player.Move.performed += OnMovePerformed;
            _playerInput.Player.Move.canceled += OnMoveCanceled;
            _playerInput.Player.Run.performed += OnRunPerformed;
            _playerInput.Player.Run.canceled += OnRunCanceled;
            _playerInput.Player.Attack.performed += OnAttackPerformed;
            _playerInput.Player.Jump.performed += OnJumpPerformed;
        }

        public void Dispose()
        {
            if (_playerInput == null) return;

            _playerInput.Player.Move.performed -= OnMovePerformed;
            _playerInput.Player.Move.canceled -= OnMoveCanceled;
            _playerInput.Player.Run.performed -= OnRunPerformed;
            _playerInput.Player.Run.canceled -= OnRunCanceled;
            _playerInput.Player.Attack.performed -= OnAttackPerformed;
            _playerInput.Player.Jump.performed -= OnJumpPerformed;
            InputSystem.onDeviceChange -= OnDeviceChange;

            _playerInput.Player.Disable();
            _playerInput.Dispose();
            _playerInput = null;
            _currentControlScheme = null;
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);

            Vector2 value = ctx.ReadValue<Vector2>();
            EventBus.RaiseEvent<IInputMoveHandler>(h => h.OnMove(value));
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);
            EventBus.RaiseEvent<IInputMoveHandler>(h => h.OnMove(Vector2.zero));
        }

        private void OnRunPerformed(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);
            EventBus.RaiseEvent<IInputRunHandler>(h => h.OnRun(true));
        }

        private void OnRunCanceled(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);
            EventBus.RaiseEvent<IInputRunHandler>(h => h.OnRun(false));
        }

        private void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);
            EventBus.RaiseEvent<IInputAttackHandler>(h => h.OnAttack());
        }

        private void OnJumpPerformed(InputAction.CallbackContext ctx)
        {
            UpdateControlScheme(ctx.control?.device);
            EventBus.RaiseEvent<IInputJumpHandler>(h => h.OnJump());
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (!(device is Gamepad))
                return;

            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                SwitchControlScheme(GamepadControlScheme);
                return;
            }

            if ((change == InputDeviceChange.Removed || change == InputDeviceChange.Disconnected)
                && Gamepad.current == null)
            {
                SwitchControlScheme(KeyboardMouseControlScheme);
            }
        }

        private void ApplyInitialControlScheme()
        {
            if (Gamepad.current != null)
            {
                SwitchControlScheme(GamepadControlScheme);
                return;
            }

            SwitchControlScheme(KeyboardMouseControlScheme);
        }

        private void UpdateControlScheme(InputDevice device)
        {
            if (device is Gamepad)
            {
                SwitchControlScheme(GamepadControlScheme);
                return;
            }

            if (device is Keyboard || device is Mouse)
            {
                SwitchControlScheme(KeyboardMouseControlScheme);
            }
        }

        private void SwitchControlScheme(string controlScheme)
        {
            if (_playerInput == null || _currentControlScheme == controlScheme)
                return;

            _currentControlScheme = controlScheme;
            _playerInput.bindingMask = InputBinding.MaskByGroup(controlScheme);

            if (controlScheme == GamepadControlScheme)
            {
                _playerInput.devices = Gamepad.current != null
                    ? new InputDevice[] { Gamepad.current }
                    : null;
                return;
            }

            List<InputDevice> devices = new List<InputDevice>(2);
            if (Keyboard.current != null)
                devices.Add(Keyboard.current);
            if (Mouse.current != null)
                devices.Add(Mouse.current);

            _playerInput.devices = devices.Count > 0
                ? devices.ToArray()
                : null;
        }
    }
}
