using EventBusSystem;

namespace Input
{
    public interface IInputBlockHandler : IGlobalSubscriber
    {
        void OnBlock(bool held);
    }
}
