using EventBusSystem;

namespace Input
{
    public interface IInputJumpHandler : IGlobalSubscriber
    {
        void OnJump();
    }
}
