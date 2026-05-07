using EventBusSystem;

namespace Input
{
    public interface IInputDodgeHandler : IGlobalSubscriber
    {
        void OnDodge();
    }
}
