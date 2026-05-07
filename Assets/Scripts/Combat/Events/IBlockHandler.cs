using Combat.Stats;
using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IBlockHandler : IGlobalSubscriber
    {
        void OnBlocked(GameObject defender, DamageInfo info, float staminaSpent, bool blockBroken);
    }
}
