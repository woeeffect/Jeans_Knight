using Character;
using UnityEngine;

namespace StateMachine.States
{
    public class JumpState : IState
    {
        private readonly HeroController _hero;

        public JumpState(HeroController hero)
        {
            _hero = hero;
        }

        private bool _hasLeftGround;

        public void Enter()
        {
            _hasLeftGround = false;
            _hero.VerticalVelocity = _hero.JumpForce;
        }

        public void Exit() { }

        public void Update()
        {
            Vector3 direction = new Vector3(_hero.MoveDirection.x, 0f, _hero.MoveDirection.y).normalized;
            _hero.CharacterController.Move(direction * (_hero.WalkSpeed * Time.deltaTime));

            if (!_hasLeftGround)
            {
                if (!_hero.IsGrounded)
                    _hasLeftGround = true;
                return;
            }

            if (_hero.IsGrounded && _hero.VerticalVelocity <= 0f)
            {
                IState returnState = _hero.StateMachine.PreviousState ?? _hero.IdleState;
                _hero.StateMachine.ChangeState(returnState);
            }
        }
    }
}
