using EventBusSystem;

namespace Input
{
    public interface IInputAttackHandler : IGlobalSubscriber
    {
        void OnAttack();
    }
}
