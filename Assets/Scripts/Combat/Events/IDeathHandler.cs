using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IDeathHandler : IGlobalSubscriber
    {
        void OnDeath(GameObject victim);
    }
}
