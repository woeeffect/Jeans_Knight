# Как добавить новое состояние персонажа

## Шаблон состояния

```csharp
using Character;
using StateMachine;

namespace Combat.States   // или StateMachine.States для базовых
{
    public class MyNewState : IState
    {
        private readonly HeroController _hero;

        public MyNewState(HeroController hero)
        {
            _hero = hero;
            // Получи нужные компоненты здесь, не в Enter()
            // var component = hero.GetComponent<SomeComponent>();
        }

        public void Enter()
        {
            // Вызывается один раз при входе в состояние
        }

        public void Exit()
        {
            // Вызывается один раз при выходе из состояния
            // Обязательно сбрасывай всё что включил в Enter()
        }

        public void Update()
        {
            // Вызывается каждый кадр
            // Здесь можно вызвать _hero.StateMachine.ChangeState(...)
        }
    }
}
```

---

## Шаги интеграции в HeroController

### 1. Добавить свойство-состояние

```csharp
// HeroController.cs — в блок "States"
public MyNewState MyNewState { get; private set; }
```

### 2. Создать экземпляр в Awake()

```csharp
// HeroController.Awake()
MyNewState = new MyNewState(this);
```

### 3. Добавить переход

В `ProcessBufferedInput()` или прямо из другого состояния:
```csharp
_stateMachine.ChangeState(MyNewState);
```

### 4. Прерываемость

Если состояние должно прерываться — убедись что `CanStartCombatAction()` его учитывает:
```csharp
// HeroController.cs — CanStartCombatAction()
if (current == MyNewState) return false; // нельзя прервать
```

---

## Примеры существующих состояний

| Состояние | Что изучить |
|---|---|
| `DodgeState` | Движение по таймеру + AnimationCurve + iframes |
| `HitReactionState` | Таймер с фиксированным выходом |
| `BlockState` | Минимальное: только вкл/выкл контроллера и аниматора |
| `AttackState` | Сложные фазы + делегирование в подсистемы |
