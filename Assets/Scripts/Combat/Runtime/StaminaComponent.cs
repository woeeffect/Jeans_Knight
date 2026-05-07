using Combat.Stats;
using UnityEngine;

namespace Combat.Runtime
{
    public class StaminaComponent : MonoBehaviour
    {
        [SerializeField] private CharacterStatsSO _stats;
        [SerializeField, Tooltip("Seconds to wait after last consumption before regen resumes.")]
        private float _regenDelay = 0.5f;

        private float _max;
        private float _current;
        private float _regen;
        private float _regenSuppressedUntil;

        public float Current => _current;
        public float Max => _max;
        public bool HasAny => _current > 0f;

        private void Awake()
        {
            _max = _stats != null ? _stats.MaxStamina : 100f;
            _regen = _stats != null ? _stats.StaminaRegenPerSec : 15f;
            _current = _max;
        }

        private void Update()
        {
            if (Time.time < _regenSuppressedUntil || _current >= _max)
                return;
            _current = Mathf.Min(_max, _current + _regen * Time.deltaTime);
        }

        public bool TryConsume(float amount)
        {
            if (amount <= 0f) return true;
            if (_current < amount) return false;
            _current -= amount;
            _regenSuppressedUntil = Time.time + _regenDelay;
            return true;
        }

        public float ConsumeUpTo(float amount)
        {
            if (amount <= 0f) return 0f;
            float consumed = Mathf.Min(_current, amount);
            _current -= consumed;
            _regenSuppressedUntil = Time.time + _regenDelay;
            return consumed;
        }
    }
}
