using Combat.Events;
using EventBusSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Combat.Runtime
{
    public class VitalsDebugLogger : MonoBehaviour, IHealthChangedHandler
    {
        [Header("References")]
        [SerializeField] private HealthComponent _health;
        [SerializeField] private StaminaComponent _stamina;
        [SerializeField] private Text _hpText;
        [SerializeField] private Text _staminaText;

        [Header("Debug")]
        [SerializeField] private bool _logToConsole = true;

        private float _lastStaminaValue = -1f;

        private void Awake()
        {
            if (_health == null)
                _health = FindAnyObjectByType<HealthComponent>();

            if (_stamina == null)
                _stamina = FindAnyObjectByType<StaminaComponent>();
        }

        private void OnEnable()
        {
            EventBus.Subscribe(this);
            RefreshAll();
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private void Update()
        {
            if (_stamina == null)
                return;

            if (Mathf.Abs(_stamina.Current - _lastStaminaValue) < 0.01f)
                return;

            _lastStaminaValue = _stamina.Current;
            SetStaminaText(_stamina.Current, _stamina.Max);

            if (_logToConsole)
                Debug.Log($"[Vitals] Stamina: {_stamina.Current:0.0}/{_stamina.Max:0.0}");
        }

        public void OnHealthChanged(GameObject owner, float currentHP, float maxHP)
        {
            if (_health != null && owner != _health.gameObject)
                return;

            SetHealthText(currentHP, maxHP);

            if (_logToConsole)
                Debug.Log($"[Vitals] HP: {currentHP:0.0}/{maxHP:0.0}");
        }

        private void RefreshAll()
        {
            if (_health != null)
                SetHealthText(_health.CurrentHP, _health.MaxHP);
            else
                SetHealthText(0f, 0f);

            if (_stamina != null)
            {
                _lastStaminaValue = _stamina.Current;
                SetStaminaText(_stamina.Current, _stamina.Max);
            }
            else
            {
                _lastStaminaValue = -1f;
                SetStaminaText(0f, 0f);
            }
        }

        private void SetHealthText(float current, float max)
        {
            if (_hpText != null)
                _hpText.text = $"HP: {current:0}/{max:0}";
        }

        private void SetStaminaText(float current, float max)
        {
            if (_staminaText != null)
                _staminaText.text = $"ST: {current:0}/{max:0}";
        }
    }
}
