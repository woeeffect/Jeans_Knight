using Input;
using Inventory.Runtime;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputService>().AsSingle();
            Container.Bind<InventoryService>().AsSingle();
        }
    }
}
