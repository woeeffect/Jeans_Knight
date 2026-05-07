using Combat.Stats;
using EventBusSystem;
using UnityEngine;

namespace Combat.Events
{
    public interface IParrySuccessHandler : IGlobalSubscriber
    {
        void OnParry(GameObject defender, GameObject attacker, DamageInfo info);
    }
}
