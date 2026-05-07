using Combat.Controllers;
using Combat.Events;
using Combat.Stats;
using EventBusSystem;
using UnityEngine;

namespace Combat.Runtime
{
    public class HealthComponent : MonoBehaviour, IDamageable
    {
        [SerializeField] private CharacterStatsSO _stats;
        [SerializeField] private float _initialHPOverride = -1f;

        private float _maxHP;
        private float _currentHP;
        private BlockController _blockController;
        private EquipmentController _equipment;

        public GameObject GameObject => gameObject;
        public bool IsDead => _currentHP <= 0f;
        public float CurrentHP => _currentHP;
        public float MaxHP => _maxHP;
        public bool IsInvincible { get; set; }

        public System.Func<DamageInfo, DamageInfo> PreDamageFilter;

        private void Awake()
        {
            _maxHP = _stats != null ? _stats.MaxHP : 100f;
            _currentHP = _initialHPOverride > 0f ? _initialHPOverride : _maxHP;
            _blockController = GetComponent<BlockController>();
            _equipment = GetComponent<EquipmentController>();
        }

        public void ApplyDamage(DamageInfo info)
        {
            if (IsDead || IsInvincible) return;

            if (_blockController != null && _blockController.IsActive)
            {
                BlockResult result = _blockController.HandleIncomingDamage(info);
                if (result.Outcome == BlockOutcome.Parried || result.Outcome == BlockOutcome.Blocked)
                    return;
                info.Amount = result.DamagePassed;
            }

            // Flat armor mitigation.
            if (_equipment != null)
            {
                float defense = _equipment.GetTotalDefense(info.Type);
                info.Amount = Mathf.Max(0f, info.Amount - defense);
            }

            if (PreDamageFilter != null)
                info = PreDamageFilter(info);

            if (info.Amount <= 0f) return;

            _currentHP = Mathf.Max(0f, _currentHP - info.Amount);

            EventBus.RaiseEvent<IDamageReceivedHandler>(h => h.OnDamageReceived(gameObject, info, _currentHP));
            EventBus.RaiseEvent<IHealthChangedHandler>(h => h.OnHealthChanged(gameObject, _currentHP, _maxHP));

            if (_currentHP <= 0f)
                EventBus.RaiseEvent<IDeathHandler>(h => h.OnDeath(gameObject));
        }

        private void OnDestroy() => PreDamageFilter = null;

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            _currentHP = Mathf.Min(_maxHP, _currentHP + amount);
            EventBus.RaiseEvent<IHealthChangedHandler>(h => h.OnHealthChanged(gameObject, _currentHP, _maxHP));
        }
    }
}
