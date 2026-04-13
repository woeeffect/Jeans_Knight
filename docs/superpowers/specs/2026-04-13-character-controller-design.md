# Character Controller System Design

## Context

Jeans_Knight needs a character control system for its Knight character. The game uses a Hades-style isometric top-down camera. Currently the project has: New Input System v1.18.0, custom EventBus, Zenject (unused), Cinemachine, UniTask, DOTween, and Knight FBX models with 4 animations (idle, walk, run, attack).

## Requirements

- WASD movement + gamepad left stick
- Shift to run + gamepad left trigger/bumper
- LMB attack + gamepad right trigger/X
- Space jump + gamepad south button (A/Cross)
- Hades-style fixed isometric camera (Cinemachine)
- Character rotates toward movement direction
- Unity CharacterController for movement
- Event Bus for input distribution
- Code-based State Machine for behavior/animation
- Zenject DI for wiring services

## Architecture

### 1. Input Layer

**PlayerInputActions.inputactions** — Unity Input Actions asset defining:

| Action | Type | Keyboard/Mouse | Gamepad |
|--------|------|-----------------|---------|
| Move | Value (Vector2) | WASD | Left Stick |
| Run | Button | Left Shift | Left Trigger / Left Bumper |
| Attack | Button | LMB | Right Trigger / X |
| Jump | Button | Space | South Button (A / Cross) |

**InputService** (plain C# class, not MonoBehaviour):
- Creates and owns `PlayerInputActions` instance
- Subscribes to performed/canceled callbacks
- Raises EventBus events on input changes
- Implements Zenject `IInitializable`, `IDisposable` (no ITickable — input is callback-based, no polling needed)
- Enables action map in `Initialize()`, disables in `Dispose()`

**EventBus subscriber interfaces:**

```csharp
public interface IInputMoveHandler : IGlobalSubscriber {
    void OnMove(Vector2 direction);
}
public interface IInputRunHandler : IGlobalSubscriber {
    void OnRun(bool isRunning);
}
public interface IInputAttackHandler : IGlobalSubscriber {
    void OnAttack();
}
public interface IInputJumpHandler : IGlobalSubscriber {
    void OnJump();
}
```

**Data flow:** `Input System -> InputService -> EventBus -> HeroController`

### 2. State Machine

**StateMachine** — manages current state and transitions:

```csharp
public class StateMachine {
    IState CurrentState { get; }
    IState PreviousState { get; } // tracked for return-to-previous logic
    void ChangeState(IState newState); // sets PreviousState, calls Exit() then Enter()
    void Update(); // delegates to CurrentState.Update()
}
```

**IState interface:**

```csharp
public interface IState {
    void Enter();
    void Exit();
    void Update();
}
```

**States:**

| State | Animation | Behavior |
|-------|-----------|----------|
| IdleState | idle | No movement input, character stands still |
| WalkState | walk | Movement input present, moves at walkSpeed |
| RunState | run | Movement input + Shift held, moves at runSpeed |
| AttackState | attack | LMB pressed, plays attack animation, returns to previous state on finish |
| JumpState | (none yet) | Space pressed, placeholder for future jump animation |

**Transitions:**
- Idle <-> Walk: movement input present/absent
- Walk <-> Run: Shift pressed/released while moving
- Idle/Walk/Run -> Attack: LMB pressed (interrupts movement states only)
- Idle/Walk/Run -> Jump: Space pressed (interrupts movement states only)
- AttackState is non-interruptible — input is ignored until animation completes
- JumpState is non-interruptible — input is ignored until landing
- Attack/Jump -> previous state: on completion. StateMachine tracks `PreviousState` property for return.

**State priority (highest first):** Attack > Jump > Run > Walk > Idle

**Gravity:** CharacterController does not apply gravity automatically. HeroController maintains a `verticalVelocity` field, applies gravity (`Physics.gravity.y * deltaTime`) each frame, and checks `CharacterController.isGrounded` to reset it.

**Animator parameters:**

| Parameter | Type | Set By |
|-----------|------|--------|
| IsMoving | Bool | IdleState (false), WalkState/RunState (true) |
| IsRunning | Bool | RunState (true), others (false) |
| Attack | Trigger | AttackState on Enter() |
| Speed | Float | WalkState/RunState (normalized 0-1) |

Each state sets Animator parameters on Enter(). Animator Controller handles only visuals.

### 3. HeroController

**HeroController** (MonoBehaviour) — main component on the character prefab:
- Implements `IInputMoveHandler`, `IInputRunHandler`, `IInputAttackHandler`, `IInputJumpHandler`
- Subscribes to EventBus on Enable, unsubscribes on Disable
- Owns `StateMachine` instance, creates all states
- References `UnityEngine.CharacterController` and `Animator` components
- Stores input data (moveDirection, isRunning) for states to read
- Rotates character toward movement direction (smooth rotation via `Quaternion.Slerp`)
- Applies gravity each frame via `verticalVelocity` field
- States access HeroController for movement/animation
- Does not require Zenject constructor injection — uses EventBus static API directly

**Serialized configuration fields:**

| Field | Type | Default | Description |
|-------|------|---------|-------------|
| walkSpeed | float | 3.0 | Walk movement speed |
| runSpeed | float | 6.0 | Run movement speed |
| rotationSpeed | float | 10.0 | Smooth rotation speed |
| gravity | float | -15.0 | Custom gravity value |
| jumpForce | float | 5.0 | Upward jump velocity |

### 4. Zenject Integration

**GameInstaller** (MonoInstaller on SceneContext):
- Binds `InputService` as `IInitializable, IDisposable` (non-tickable)
- SceneContext placed in Game scene

### 5. Camera (Hades-style)

- **Cinemachine 3.x** (project uses v3.1.6): `CinemachineCamera` component (not the legacy `CinemachineVirtualCamera`)
- Body: `CinemachineFollow` with offset ~(0, 12, -8) for ~55 degree angle
- Aim: `CinemachineRotationComposer` or no aim (fixed rotation)
- Follow target: character transform
- No mouse-controlled rotation
- Camera is configured purely in the scene — no custom C# script needed
- Isometric/semi-isometric perspective

### 6. File Structure

```
Assets/Scripts/
+-- EventBus/
|   +-- EventBusSystem/ (existing)
+-- Input/
|   +-- InputService.cs
|   +-- IInputMoveHandler.cs
|   +-- IInputRunHandler.cs
|   +-- IInputAttackHandler.cs
|   +-- IInputJumpHandler.cs
+-- StateMachine/
|   +-- IState.cs
|   +-- StateMachine.cs
|   +-- States/
|       +-- IdleState.cs
|       +-- WalkState.cs
|       +-- RunState.cs
|       +-- AttackState.cs
|       +-- JumpState.cs
+-- Character/
|   +-- HeroController.cs
+-- Installers/
    +-- GameInstaller.cs
```

Plus `Assets/InputActions/PlayerInputActions.inputactions` (generated asset).

## Verification

1. Open Game scene, ensure SceneContext with GameInstaller is present
2. Place Knight prefab with HeroController, CharacterController, Animator components
3. Play mode: WASD moves character, Shift toggles walk/run, LMB triggers attack animation, Space triggers jump state
4. Connect gamepad: left stick moves, triggers/buttons map correctly
5. Verify state transitions: idle when no input, walk/run on movement, attack interrupts, returns to previous state after attack ends
6. Camera follows character at fixed isometric angle
