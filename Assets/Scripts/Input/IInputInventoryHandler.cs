using EventBusSystem;

namespace Input
{
    public interface IInputInventoryHandler : IGlobalSubscriber
    {
        void OnOpenInventory();
    }
}
