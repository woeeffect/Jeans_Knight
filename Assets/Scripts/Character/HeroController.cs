using Combat.Controllers;
using Combat.Runtime;
using Combat.States;
using Combat.Stats;
using EventBusSystem;
using Input;
using StateMachine.States;
using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Animator))]
    public class HeroController : MonoBehaviour,
        IInputMoveHandler,
        IInputRunHandler,
        IInputAttackHandler,
        IInputBlockHandler,
        IInputDodgeHandler,
        IStunnable
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _runSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 10f;

        [Header("Physics")]
        [SerializeField] private float _gravity = -15f;

        [Header("Dodge")]
        [SerializeField] private float _dodgeSpeed = 9f;
        [SerializeField] private float _dodgeDuration = 0.28f;
        [SerializeField] private float _dodgeStaminaCost = 20f;
        [SerializeField] private AnimationCurve _dodgeSpeedCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [Header("Stats")]
        [SerializeField] private CharacterStatsSO _stats;

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        [SerializeField] private HealthComponent _health;
        [SerializeField] private StaminaComponent _stamina;
        [SerializeField] private ComboController _comboController;
        [SerializeField] private WeaponHandler _weaponHandler;
        [SerializeField] private EquipmentController _equipmentController;
        
        private StateMachine.StateMachine _stateMachine;

        private Vector2 _moveDirection;
        private bool _isRunning;
        private float _verticalVelocity;
        private bool _attackRequested;
        private bool _dodgeRequested;
        private bool _blockHeld;

        // States
        public IdleState IdleState { get; private set; }
        public WalkState WalkState { get; private set; }
        public RunState RunState { get; private set; }
        public AttackState AttackState { get; private set; }
        public BlockState BlockState { get; private set; }
        public DodgeState DodgeState { get; private set; }
        public HitReactionState HitReactionState { get; private set; }

        // Public accessors
        public StateMachine.StateMachine StateMachine => _stateMachine;
        public CharacterController CharacterController => _characterController;
        public Animator Animator => _animator;
        public HealthComponent Health => _health;
        public StaminaComponent Stamina => _stamina;
        public ComboController ComboController => _comboController;
        public WeaponHandler WeaponHandler => _weaponHandler;
        public EquipmentController EquipmentController => _equipmentController;
        public CharacterStatsSO Stats => _stats;
        public Vector2 MoveDirection => _moveDirection;
        public bool IsRunning => _isRunning;
        public bool IsBlockHeld => _blockHeld;
        public float WalkSpeed => _walkSpeed;
        public float RunSpeed => _runSpeed;
        public float RotationSpeed => _rotationSpeed;
        public bool IsGrounded => _characterController.isGrounded;
        public float DodgeSpeed => _dodgeSpeed;
        public float DodgeDuration => _dodgeDuration;
        public float DodgeStaminaCost => _dodgeStaminaCost;
        public AnimationCurve DodgeSpeedCurve => _dodgeSpeedCurve;

        public float VerticalVelocity
        {
            get => _verticalVelocity;
            set => _verticalVelocity = value;
        }

        private void Awake()
        {
            IdleState = new IdleState(this);
            WalkState = new WalkState(this);
            RunState = new RunState(this);
            AttackState = new AttackState(this);
            BlockState = new BlockState(this);
            DodgeState = new DodgeState(this);
            HitReactionState = new HitReactionState(this);

            _stateMachine = new StateMachine.StateMachine();
            _stateMachine.ChangeState(IdleState);
        }

        private void OnEnable() => EventBus.Subscribe(this);
        private void OnDisable() => EventBus.Unsubscribe(this);

        private void Update()
        {
            ApplyGravity();
            ProcessBufferedInput();
            _stateMachine.Update();
        }

        private void ApplyGravity()
        {
            if (_characterController.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;//magic variable
            else
                _verticalVelocity += _gravity * Time.deltaTime;

            _characterController.Move(new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f));
        }

        private bool CanStartCombatAction()
        {
            var current = _stateMachine.CurrentState;
            if (current == HitReactionState) return false;
            if (current == DodgeState) return false;
            if (current == AttackState)
                return AttackState.IsInCancellablePhase;
            return true;
        }

        private bool CanAdvanceCombo()
        {
            var current = _stateMachine.CurrentState;
            if (current != AttackState) return true;
            return AttackState.Phase == AttackPhase.Recovery;
        }

        public void OnMove(Vector2 direction) => _moveDirection = direction;
        public void OnRun(bool isRunning) => _isRunning = isRunning;
        public void OnAttack() => _attackRequested = true;
        public void OnBlock(bool held) => _blockHeld = held;
        public void OnDodge() => _dodgeRequested = true;

        private void ProcessBufferedInput()
        {
            if (_dodgeRequested)
            {
                _dodgeRequested = false;
                if (CanStartCombatAction())
                    _stateMachine.ChangeState(DodgeState);
            }

            if (_attackRequested)
            {
                _attackRequested = false;
                if (CanAdvanceCombo() && _stateMachine.CurrentState != HitReactionState
                    && _stateMachine.CurrentState != DodgeState)
                    _stateMachine.ChangeState(AttackState);
            }

            if (_stateMachine.CurrentState == BlockState)
            {
                if (!_blockHeld)
                    _stateMachine.ChangeState(IdleState);
            }
            else if (_blockHeld && CanStartCombatAction())
            {
                _stateMachine.ChangeState(BlockState);
            }
        }

        public void Stun(float duration)
        {
            HitReactionState.EnterStun(duration);
            _stateMachine.ChangeState(HitReactionState);
        }

        // Called by Animation Events on the attack clips — do not rename.
        public void AnimEvent_WindupEnd() => AttackState.NotifyWindupEnd();
        public void AnimEvent_HitStart() => AttackState.NotifyHitStart();
        public void AnimEvent_HitEnd() => AttackState.NotifyHitEnd();
        public void AnimEvent_RecoveryEnd() => AttackState.NotifyRecoveryEnd();
    }
}
