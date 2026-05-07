using Character;
using Combat.Data;
using Combat.Runtime;
using Combat.Stats;
using StateMachine;
using UnityEngine;

namespace Combat.States
{
    public enum AttackPhase
    {
        None,
        Windup,
        Hit,
        Recovery,
        Done
    }

    public class AttackState : IState
    {
        private readonly HeroController _hero;

        private AttackActionSO _action;
        private WeaponDataSO _weapon;
        private DamageInfo _damage;
        private int _comboIndex;

        private AttackPhase _phase;
        private float _phaseFallbackTimer;
        private bool _animEventReceivedForPhase;

        public AttackPhase Phase => _phase;
        public bool IsInCancellablePhase => _phase == AttackPhase.Windup || _phase == AttackPhase.Recovery;

        public AttackState(HeroController hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            _weapon = _hero.WeaponHandler != null ? _hero.WeaponHandler.CurrentWeapon : null;
            _action = _hero.ComboController != null ? _hero.ComboController.GetNextAction(_weapon) : null;

            if (_weapon == null || _action == null)
            {
                _phase = AttackPhase.Done;
                _hero.StateMachine.ChangeState(_hero.IdleState);
                return;
            }

            _comboIndex = _hero.ComboController.CurrentIndex;

            float staminaCost = _action.StaminaCostOverride > 0f
                ? _action.StaminaCostOverride
                : _weapon.StaminaCost;
            if (_hero.Stamina != null && staminaCost > 0f)
                _hero.Stamina.ConsumeUpTo(staminaCost);

            _damage = DamageCalculator.Compute(_weapon, _action, _hero.Stats, _comboIndex, _hero.gameObject);

            _hero.Animator.SetBool(AnimatorParams.IsMoving, false);
            _hero.Animator.SetBool(AnimatorParams.IsRunning, false);
            _hero.Animator.SetInteger(AnimatorParams.ComboIndex, _comboIndex);
            _hero.Animator.SetInteger(AnimatorParams.WeaponCategory, (int)_weapon.Category);
            if (!string.IsNullOrEmpty(_action.AnimationTrigger))
                _hero.Animator.SetTrigger(_action.AnimationTrigger);

            EnterPhase(AttackPhase.Windup);
        }

        public void Exit()
        {
            _hero.WeaponHandler?.HandleHitEnd();
            _phase = AttackPhase.Done;
        }

        public void Update()
        {
            if (_phase == AttackPhase.None || _phase == AttackPhase.Done)
                return;

            if (_animEventReceivedForPhase) return;

            _phaseFallbackTimer -= Time.deltaTime;
            if (_phaseFallbackTimer <= 0f)
                AdvancePhase();
        }

        public void NotifyWindupEnd()
        {
            if (_phase == AttackPhase.Windup) { _animEventReceivedForPhase = true; AdvancePhase(); }
        }

        public void NotifyHitStart()
        {
            if (_phase == AttackPhase.Windup) { _animEventReceivedForPhase = true; AdvancePhase(); }
        }

        public void NotifyHitEnd()
        {
            if (_phase == AttackPhase.Hit) { _animEventReceivedForPhase = true; AdvancePhase(); }
        }

        public void NotifyRecoveryEnd()
        {
            if (_phase == AttackPhase.Recovery) { _animEventReceivedForPhase = true; AdvancePhase(); }
        }

        private void EnterPhase(AttackPhase phase)
        {
            _phase = phase;
            _animEventReceivedForPhase = false;

            switch (phase)
            {
                case AttackPhase.Windup:
                    _phaseFallbackTimer = _action.WindupDuration;
                    break;
                case AttackPhase.Hit:
                    _phaseFallbackTimer = _action.HitDuration;
                    _hero.WeaponHandler?.HandleHitPhase(_damage);
                    break;
                case AttackPhase.Recovery:
                    _phaseFallbackTimer = _action.RecoveryDuration;
                    _hero.WeaponHandler?.HandleHitEnd();
                    break;
            }
        }

        private void AdvancePhase()
        {
            switch (_phase)
            {
                case AttackPhase.Windup:
                    EnterPhase(AttackPhase.Hit);
                    break;
                case AttackPhase.Hit:
                    EnterPhase(AttackPhase.Recovery);
                    break;
                case AttackPhase.Recovery:
                    _phase = AttackPhase.Done;
                    _hero.WeaponHandler?.HandleHitEnd();
                    _hero.StateMachine.ChangeState(_hero.IdleState);
                    break;
            }
        }
    }
}
