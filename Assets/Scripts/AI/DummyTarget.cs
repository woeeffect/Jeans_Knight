using Combat.Data;
using Combat.Events;
using Combat.Runtime;
using Combat.Stats;
using DG.Tweening;
using EventBusSystem;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(HealthComponent))]
    public class DummyTarget : MonoBehaviour,
        IDamageReceivedHandler,
        IDeathHandler,
        IStunnable
    {
        [Header("Visuals")]
        [SerializeField] private Renderer _targetRenderer;
        [SerializeField] private Color _hitFlashColor = Color.red;
        [SerializeField] private Color _stunColor = new Color(1f, 0.85f, 0f);
        [SerializeField] private float _hitFlashDuration = 0.12f;
        [SerializeField] private float _destroyDelayOnDeath = 2f;

        [Header("Debug Attack")]
        [SerializeField] private HealthComponent _playerHealth;
        [SerializeField] private float _debugAttackDamage = 15f;
        [SerializeField] private DamageType _debugAttackType = DamageType.Physical;

        private HealthComponent _health;
        private MaterialPropertyBlock _mpb;
        private Color _baseColor;
        private Tween _flashTween;
        private Tween _stunTween;
        private float _stunUntil;

        public bool IsStunned => Time.time < _stunUntil;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
            if (_targetRenderer == null)
                _targetRenderer = GetComponentInChildren<Renderer>();
            _mpb = new MaterialPropertyBlock();
            _baseColor = Color.white;
        }

        private void OnEnable() => EventBus.Subscribe(this);
        private void OnDisable() => EventBus.Unsubscribe(this);

        public void OnDamageReceived(GameObject victim, DamageInfo info, float remainingHP)
        {
            if (victim != gameObject) return;
            Debug.Log($"[Dummy] hit for {info.Amount:F1} {info.Type} ({info.WeaponCategory}), HP: {remainingHP:F1}/{_health.MaxHP:F1}");
            FlashHit();
        }

        public void OnDeath(GameObject victim)
        {
            if (victim != gameObject) return;
            Debug.Log("[Dummy] dead");
            Destroy(gameObject, _destroyDelayOnDeath);
        }

        public void Stun(float seconds)
        {
            _stunUntil = Mathf.Max(_stunUntil, Time.time + seconds);
            Debug.Log($"[Dummy] stunned for {seconds:F2}s");
            FlashStun(seconds);
        }

        [ContextMenu("Debug: Attack Player")]
        public void DebugAttackPlayer()
        {
            if (_playerHealth == null)
            {
                _playerHealth = FindAnyObjectByType<HealthComponent>();
                if (_playerHealth == null || _playerHealth.gameObject == gameObject)
                {
                    Debug.LogWarning("[Dummy] No player HealthComponent found.");
                    return;
                }
            }
            var info = new DamageInfo(_debugAttackDamage, _debugAttackType, WeaponCategory.MeleePhysical, gameObject, 0);
            _playerHealth.ApplyDamage(info);
            Debug.Log($"[Dummy] attacks player for {_debugAttackDamage} {_debugAttackType}");
        }

        private void FlashHit()
        {
            if (_targetRenderer == null) return;
            _flashTween?.Kill();
            _stunTween?.Kill();
            SetColor(_hitFlashColor);
            _flashTween = DOVirtual.DelayedCall(_hitFlashDuration, RestoreColor);
        }

        private void FlashStun(float duration)
        {
            if (_targetRenderer == null) return;
            _flashTween?.Kill();
            _stunTween?.Kill();
            SetColor(_stunColor);
            _stunTween = DOVirtual.DelayedCall(duration, RestoreColor);
        }

        private void RestoreColor() => SetColor(_baseColor);

        private void SetColor(Color color)
        {
            _targetRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_Color", color);
            _targetRenderer.SetPropertyBlock(_mpb);
        }
    }
}
