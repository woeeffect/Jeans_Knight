using EventBusSystem;

namespace Input
{
    public interface IInputRunHandler : IGlobalSubscriber
    {
        void OnRun(bool isRunning);
    }
}
