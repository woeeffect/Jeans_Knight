using Character;
using UnityEngine;

namespace StateMachine.States
{
    public class IdleState : IState
    {
        private readonly HeroController _hero;

        public IdleState(HeroController hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            _hero.Animator.SetBool(AnimatorParams.IsMoving, false);
            _hero.Animator.SetBool(AnimatorParams.IsRunning, false);
            _hero.Animator.SetFloat(AnimatorParams.Speed, 0f);
        }

        public void Exit() { }

        public void Update()
        {
            if (_hero.MoveDirection.sqrMagnitude > 0.01f)
            {
                _hero.StateMachine.ChangeState(_hero.WalkState);
            }
        }
    }
}
