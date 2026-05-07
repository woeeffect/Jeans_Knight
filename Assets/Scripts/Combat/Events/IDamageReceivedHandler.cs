using Combat.Stats;
using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IDamageReceivedHandler : IGlobalSubscriber
    {
        void OnDamageReceived(GameObject victim, DamageInfo info, float remainingHP);
    }
}
