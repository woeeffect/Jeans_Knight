using EventBusSystem;
using Input;
using StateMachine;
using StateMachine.States;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(UnityEngine.CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class HeroController : MonoBehaviour,
        IInputMoveHandler,
        IInputRunHandler,
        IInputAttackHandler,
        IInputJumpHandler
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float runSpeed = 6f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Physics")]
        [SerializeField] private float gravity = -15f;
        [SerializeField] private float jumpForce = 5f;

        private StateMachine.StateMachine _stateMachine;
        private UnityEngine.CharacterController _characterController;
        private Animator _animator;

        private Vector2 _moveDirection;
        private bool _isRunning;
        private float _verticalVelocity;
        private bool _jumpRequested;
        private bool _attackRequested;

        // States
        public IdleState IdleState { get; private set; }
        public WalkState WalkState { get; private set; }
        public RunState RunState { get; private set; }
        public AttackState AttackState { get; private set; }
        public JumpState JumpState { get; private set; }

        // Public accessors for states
        public StateMachine.StateMachine StateMachine => _stateMachine;
        public UnityEngine.CharacterController CharacterController => _characterController;
        public Animator Animator => _animator;
        public Vector2 MoveDirection => _moveDirection;
        public bool IsRunning => _isRunning;
        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float RotationSpeed => rotationSpeed;
        public float JumpForce => jumpForce;

        public bool IsGrounded => _characterController.isGrounded;

        public float VerticalVelocity
        {
            get => _verticalVelocity;
            set => _verticalVelocity = value;
        }

        private void Awake()
        {
            _characterController = GetComponent<UnityEngine.CharacterController>();
            _animator = GetComponent<Animator>();

            IdleState = new IdleState(this);
            WalkState = new WalkState(this);
            RunState = new RunState(this);
            AttackState = new AttackState(this);
            JumpState = new JumpState(this);

            _stateMachine = new StateMachine.StateMachine();
            _stateMachine.ChangeState(IdleState);
        }

        private void OnEnable()
        {
            EventBus.Subscribe(this);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe(this);
        }

        private void Update()
        {
            ApplyGravity();
            ProcessBufferedInput();
            _stateMachine.Update();
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            _characterController.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
        }

        private bool IsInterruptible()
        {
            var current = _stateMachine.CurrentState;
            return current != AttackState && current != JumpState;
        }

        // Input handlers
        public void OnMove(Vector2 direction)
        {
            _moveDirection = direction;
        }

        public void OnRun(bool isRunning)
        {
            _isRunning = isRunning;
        }

        public void OnAttack()
        {
            _attackRequested = true;
        }

        public void OnJump()
        {
            _jumpRequested = true;
        }

        private void ProcessBufferedInput()
        {
            if (_attackRequested)
            {
                _attackRequested = false;
                if (IsInterruptible())
                    _stateMachine.ChangeState(AttackState);
            }

            if (_jumpRequested)
            {
                _jumpRequested = false;
                if (IsInterruptible() && _characterController.isGrounded)
                    _stateMachine.ChangeState(JumpState);
            }
        }
    }
}
