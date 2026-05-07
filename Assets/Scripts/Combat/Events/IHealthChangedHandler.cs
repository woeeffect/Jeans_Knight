using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IHealthChangedHandler : IGlobalSubscriber
    {
        void OnHealthChanged(GameObject owner, float currentHP, float maxHP);
    }
}
