using Combat.Events;
using Combat.Runtime;
using Combat.Stats;
using EventBusSystem;
using UnityEngine;

namespace Combat.Controllers
{
    public enum BlockOutcome
    {
        NotBlocking,
        Parried,
        Blocked,
        BlockBroken
    }

    public struct BlockResult
    {
        public BlockOutcome Outcome;
        public float DamagePassed;
        public float StaminaSpent;
    }

    public class BlockController : MonoBehaviour
    {
        [SerializeField, Tooltip("Seconds after block start that count as a parry.")]
        private float _parryWindowStart = 0.1f;
        [SerializeField] private float _parryWindowEnd = 0.25f;

        [SerializeField, Tooltip("Cone (degrees) centered on forward that blocks must face to apply.")]
        private float _frontArc = 200f;

        [SerializeField, Tooltip("Duration (seconds) the attacker is stunned on a successful parry.")]
        private float _parryStunDuration = 1.25f;

        private StaminaComponent _stamina;
        private float _blockStartTime = -999f;
        private bool _active;

        public bool IsActive => _active;
        public float BlockStartTime => _blockStartTime;

        private void Awake()
        {
            _stamina = GetComponent<StaminaComponent>();
        }

        public void BeginBlock()
        {
            _active = true;
            _blockStartTime = Time.time;
        }

        public void EndBlock()
        {
            _active = false;
        }

        public bool IsInParryWindow()
        {
            if (!_active) return false;
            float elapsed = Time.time - _blockStartTime;
            return elapsed >= _parryWindowStart && elapsed <= _parryWindowEnd;
        }

        public BlockResult HandleIncomingDamage(DamageInfo info)
        {
            var result = new BlockResult { Outcome = BlockOutcome.NotBlocking, DamagePassed = info.Amount };
            if (!_active) return result;

            if (!IsAttackerInFrontArc(info.Attacker))
                return result;

            if (IsInParryWindow())
            {
                result.Outcome = BlockOutcome.Parried;
                result.DamagePassed = 0f;
                if (info.Attacker != null && info.Attacker.TryGetComponent<IStunnable>(out var stunnable))
                    stunnable.Stun(_parryStunDuration);

                EventBus.RaiseEvent<IParrySuccessHandler>(h => h.OnParry(gameObject, info.Attacker, info));
                return result;
            }

            float staminaNeeded = info.Amount;
            if (_stamina == null)
            {
                result.Outcome = BlockOutcome.Blocked;
                result.DamagePassed = 0f;
                EventBus.RaiseEvent<IBlockHandler>(h => h.OnBlocked(gameObject, info, 0f, false));
                return result;
            }

            float consumed = _stamina.ConsumeUpTo(staminaNeeded);
            result.StaminaSpent = consumed;

            if (consumed >= staminaNeeded - 0.001f)
            {
                result.Outcome = BlockOutcome.Blocked;
                result.DamagePassed = 0f;
                EventBus.RaiseEvent<IBlockHandler>(h => h.OnBlocked(gameObject, info, consumed, false));
            }
            else
            {
                float ratio = staminaNeeded > 0f ? consumed / staminaNeeded : 1f;
                result.Outcome = BlockOutcome.BlockBroken;
                result.DamagePassed = info.Amount * (1f - ratio);
                _active = false;
                EventBus.RaiseEvent<IBlockHandler>(h => h.OnBlocked(gameObject, info, consumed, true));
            }

            return result;
        }

        private bool IsAttackerInFrontArc(GameObject attacker)
        {
            if (attacker == null) return true;
            Vector3 toAttacker = attacker.transform.position - transform.position;
            toAttacker.y = 0f;
            if (toAttacker.sqrMagnitude < 0.0001f) return true;
            float angle = Vector3.Angle(transform.forward, toAttacker.normalized);
            return angle <= _frontArc * 0.5f;
        }
    }
}
