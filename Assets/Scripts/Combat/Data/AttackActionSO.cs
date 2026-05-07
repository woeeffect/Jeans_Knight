using UnityEngine;

namespace Combat.Data
{
    [CreateAssetMenu(menuName = "Jeans_Knight/Combat/Attack Action", fileName = "AttackAction")]
    public class AttackActionSO : ScriptableObject
    {
        [Header("Animation")]
        [SerializeField, Tooltip("Animator trigger name fired on Enter. Leave empty to rely on ComboIndex only.")]
        private string _animationTrigger = "Attack";

        [Header("Fallback phase timing (seconds)")]
        [Tooltip("Used when the clip has no AttackPhase animation events.")]
        [SerializeField] private float _windupDuration = 0.25f;
        [SerializeField] private float _hitDuration = 0.15f;
        [SerializeField] private float _recoveryDuration = 0.3f;

        [Header("Damage")]
        [SerializeField, Tooltip("Per-attack multiplier applied on top of combo scaling.")]
        private float _damageMultiplier = 1f;

        [SerializeField, Tooltip("Stamina consumed by this attack (overrides weapon default if > 0).")]
        private float _staminaCostOverride = 0f;

        public string AnimationTrigger => _animationTrigger;
        public float WindupDuration => _windupDuration;
        public float HitDuration => _hitDuration;
        public float RecoveryDuration => _recoveryDuration;
        public float DamageMultiplier => _damageMultiplier;
        public float StaminaCostOverride => _staminaCostOverride;
    }
}
