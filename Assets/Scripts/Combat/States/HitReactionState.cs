using Character;
using StateMachine;
using UnityEngine;

namespace Combat.States
{
    public class HitReactionState : IState
    {
        private readonly HeroController _hero;
        private float _exitTime;

        public HitReactionState(HeroController hero)
        {
            _hero = hero;
        }

        public void EnterStun(float duration)
        {
            _exitTime = Time.time + Mathf.Max(0.05f, duration);
        }

        public void Enter()
        {
            _hero.Animator.SetTrigger(AnimatorParams.Hit);
            if (_exitTime <= Time.time)
                _exitTime = Time.time + 0.4f;
        }

        public void Exit() { }

        public void Update()
        {
            if (Time.time >= _exitTime)
                _hero.StateMachine.ChangeState(_hero.IdleState);
        }
    }
}
