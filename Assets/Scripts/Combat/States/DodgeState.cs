using Character;
using StateMachine;
using UnityEngine;

namespace Combat.States
{
    public class DodgeState : IState
    {
        private readonly HeroController _hero;

        private Vector3 _dodgeDir;
        private float _timer;
        private bool _iframesActive;

        public DodgeState(HeroController hero)
        {
            _hero = hero;
        }

        public void Enter()
        {
            // Dash direction: camera-relative movement input or character forward as fallback.
            Vector2 input = _hero.MoveDirection;
            if (input.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = Camera.main != null
                    ? Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized
                    : _hero.transform.forward;
                Vector3 camRight = Camera.main != null
                    ? Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized
                    : _hero.transform.right;
                _dodgeDir = (camForward * input.y + camRight * input.x).normalized;
            }
            else
            {
                _dodgeDir = _hero.transform.forward;
            }

            // Consume stamina (best-effort, dodge proceeds even if stamina is short).
            _hero.Stamina?.ConsumeUpTo(_hero.DodgeStaminaCost);

            _timer = 0f;
            _iframesActive = true;
            if (_hero.Health != null)
                _hero.Health.IsInvincible = true;

            _hero.Animator.SetTrigger(AnimatorParams.Dodge);

            // Orient character toward dodge direction immediately.
            if (_dodgeDir.sqrMagnitude > 0.001f)
                _hero.transform.rotation = Quaternion.LookRotation(_dodgeDir);
        }

        public void Exit()
        {
            if (_iframesActive && _hero.Health != null)
                _hero.Health.IsInvincible = false;
            _iframesActive = false;
        }

        public void Update()
        {
            _timer += Time.deltaTime;

            float progress = Mathf.Clamp01(_timer / _hero.DodgeDuration);
            float speedMult = _hero.DodgeSpeedCurve.Evaluate(progress);
            Vector3 motion = _dodgeDir * (_hero.DodgeSpeed * speedMult * Time.deltaTime);
            _hero.CharacterController.Move(motion);

            if (_timer >= _hero.DodgeDuration)
            {
                _hero.StateMachine.ChangeState(_hero.IdleState);
            }
        }
    }
}
