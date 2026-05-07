using UnityEngine;

namespace Combat.Stats
{
    [CreateAssetMenu(menuName = "Jeans_Knight/Stats/Character Stats", fileName = "CharacterStats")]
    public class CharacterStatsSO : ScriptableObject
    {
        [Header("Offense")]
        [SerializeField] private float _strength = 10f;
        [SerializeField] private float _magicPower = 10f;
        [SerializeField, Tooltip("Multiplier applied per stat point: damage *= 1 + stat * statScaling")]
        private float _statScaling = 0.01f;

        [Header("Vitality")]
        [SerializeField] private float _maxHP = 100f;
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _staminaRegenPerSec = 15f;

        [Header("Block")]
        [SerializeField, Tooltip("Stamina consumed per 1 point of blocked damage")]
        private float _blockStaminaRatio = 1f;

        public float Strength => _strength;
        public float MagicPower => _magicPower;
        public float StatScaling => _statScaling;
        public float MaxHP => _maxHP;
        public float MaxStamina => _maxStamina;
        public float StaminaRegenPerSec => _staminaRegenPerSec;
        public float BlockStaminaRatio => _blockStaminaRatio;
    }
}
