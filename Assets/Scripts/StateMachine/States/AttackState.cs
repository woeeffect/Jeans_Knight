using Character;
using UnityEngine;

namespace StateMachine.States
{
    public class AttackState : IState
    {
        private readonly HeroController _hero;
        private bool _animationStarted;

        public AttackState(HeroController hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            _animationStarted = false;
            _hero.Animator.SetBool(AnimatorParams.IsMoving, false);
            _hero.Animator.SetBool(AnimatorParams.IsRunning, false);
            _hero.Animator.SetTrigger(AnimatorParams.Attack);
        }

        public void Exit() { }

        public void Update()
        {
            AnimatorStateInfo stateInfo = _hero.Animator.GetCurrentAnimatorStateInfo(0);

            if (!_animationStarted)
            {
                if (stateInfo.IsTag("Attack"))
                    _animationStarted = true;
                return;
            }

            // Attack finished: either normalizedTime reached 1, or Animator already
            // transitioned out of the Attack state (HasExitTime fired)
            if (!stateInfo.IsTag("Attack") || stateInfo.normalizedTime >= 1f)
            {
                IState returnState = _hero.StateMachine.PreviousState ?? _hero.IdleState;
                _hero.StateMachine.ChangeState(returnState);
            }
        }
    }
}
