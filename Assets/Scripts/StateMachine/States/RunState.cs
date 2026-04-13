using Character;
using UnityEngine;

namespace StateMachine.States
{
    public class RunState : IState
    {
        private readonly HeroController _hero;

        public RunState(HeroController hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            _hero.Animator.SetBool(AnimatorParams.IsMoving, true);
            _hero.Animator.SetBool(AnimatorParams.IsRunning, true);
        }

        public void Exit() { }

        public void Update()
        {
            if (_hero.MoveDirection.sqrMagnitude < 0.01f)
            {
                _hero.StateMachine.ChangeState(_hero.IdleState);
                return;
            }

            if (!_hero.IsRunning)
            {
                _hero.StateMachine.ChangeState(_hero.WalkState);
                return;
            }

            MoveAndRotate(_hero.RunSpeed);
            _hero.Animator.SetFloat(AnimatorParams.Speed, 1f);
        }

        private void MoveAndRotate(float speed)
        {
            Vector3 direction = new Vector3(_hero.MoveDirection.x, 0f, _hero.MoveDirection.y).normalized;

            _hero.CharacterController.Move(direction * (speed * Time.deltaTime));

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                _hero.transform.rotation = Quaternion.Slerp(
                    _hero.transform.rotation, targetRotation, _hero.RotationSpeed * Time.deltaTime);
            }
        }
    }
}
