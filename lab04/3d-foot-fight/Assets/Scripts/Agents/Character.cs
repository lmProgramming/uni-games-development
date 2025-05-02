using JetBrains.Annotations;
using States.Fighting;
using States.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Agents
{
    [RequireComponent(typeof(CharacterController))]
    public class Character : MonoBehaviour
    {
        [Header("Movement Settings")] [SerializeField]
        public float moveSpeed = 7f;

        [SerializeField] public float airMoveSpeedMultiplier = 0.8f;
        [SerializeField] public float jumpHeight = 2.0f;
        [SerializeField] public float gravityValue = -19.62f;

        [Header("Look Settings")] [SerializeField]
        private float lookSpeed = 2.0f;

        [SerializeField] private float lookXLimit = 80.0f;

        [Header("Ground Check Settings")]
        [Tooltip("Layer Mask for sphere casts. Assign the Player object to a Layer, then Ignore that layer here.")]
        [SerializeField]
        private LayerMask groundMask = ~0; // Default to everything

        [Tooltip("How far below the character's center the ground check starts.")] [SerializeField]
        private float groundCheckYOffset = -0.85f;

        [Tooltip("Radius of the ground check sphere.")] [SerializeField]
        private float groundCheckRadius = 0.4f;

        [Tooltip("Distance the spherecast checks downwards.")] [SerializeField]
        private float groundCheckDistance = 0.3f;

        [field: SerializeField] public Damageable Damageable { get; private set; }

        [CanBeNull] [field: SerializeField] public Weapon CurrentWeapon { get; private set; }

        private float _cameraRotationX;

        private bool _jumpRequested;


        public CharacterController Controller { get; private set; }
        public Camera PlayerCamera { get; private set; }
        public Vector2 LookDirection { get; private set; } = Vector2.zero;
        public Vector2 MoveDirection { get; private set; } = Vector2.zero;
        public bool JumpRequested { get; private set; }
        public bool AttackRequested { get; private set; }
        public float VerticalVelocity { get; set; }

        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            Controller = GetComponent<CharacterController>();
            PlayerCamera = Camera.main;

            if (PlayerCamera == null)
                Debug.LogError("Main Camera not found! Ensure a camera is tagged 'MainCamera'.", this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _movementStateMachine = new MovementStateMachine();

            Grounded = new Grounded(this, _movementStateMachine);
            Airborne = new Airborne(this, _movementStateMachine);


            _fightingStateMachine = new FightingStateMachine();

            Ready = new Ready(this, _fightingStateMachine);

            SwitchWeapon(GetComponentInChildren<Weapon>());
        }

        private void Start()
        {
            var grounded = new Grounded(this, _movementStateMachine);
            _movementStateMachine.Initialize(grounded);

            var ready = new Ready(this, _fightingStateMachine);
            _fightingStateMachine.Initialize(ready);
        }

        private void Update()
        {
            ProcessLook();

            _movementStateMachine.CurrentState.HandleInput();
            _movementStateMachine.CurrentState.LogicUpdate();

            _fightingStateMachine.CurrentState.HandleInput();
            _fightingStateMachine.CurrentState.LogicUpdate();

            JumpRequested = false;
            AttackRequested = false;
        }

        private void FixedUpdate()
        {
            PerformGroundCheck();

            _movementStateMachine.CurrentState.PhysicsUpdate();

            _movementStateMachine.CurrentState.PostPhysicsUpdate();
        }

        private void OnDrawGizmosSelected()
        {
            PerformGroundCheck();

            var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = IsGrounded ? transparentGreen : transparentRed;

            var spherePosition = new Vector3(transform.position.x, transform.position.y + groundCheckYOffset,
                transform.position.z);
            Gizmos.DrawSphere(spherePosition, groundCheckRadius);

            Gizmos.DrawWireSphere(spherePosition + Vector3.down * groundCheckDistance, groundCheckRadius);
            Gizmos.DrawLine(spherePosition, spherePosition + Vector3.down * groundCheckDistance);
        }

        private void ProcessLook()
        {
            if (!PlayerCamera) return;

            var lookX = LookDirection.x * lookSpeed * Time.deltaTime;
            var lookY = LookDirection.y * lookSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * lookX);

            _cameraRotationX -= lookY;
            _cameraRotationX = Mathf.Clamp(_cameraRotationX, -lookXLimit, lookXLimit);

            PlayerCamera.transform.localRotation = Quaternion.Euler(_cameraRotationX, 0f, 0f);
        }

        private void PerformGroundCheck()
        {
            var spherePosition = new Vector3(transform.position.x, transform.position.y + groundCheckYOffset,
                transform.position.z);

            IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundMask,
                QueryTriggerInteraction.Ignore);

            //RaycastHit hit;
            //IsGrounded = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, groundCheckDistance,
            //    groundMask, QueryTriggerInteraction.Ignore);
            //if (IsGrounded)
            //{
            //    /* Process hit.normal for the slope */
            //}
            //if (!IsGrounded) IsGrounded = Controller.isGrounded;
        }

        private void SwitchWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
        }

        public Swinging GetSwingingState(float swingTime)
        {
            return new Swinging(this, _fightingStateMachine, swingTime);
        }

        public Recovery GetRecoveryState(float recoveryTime)
        {
            return new Recovery(this, _fightingStateMachine, recoveryTime);
        }

        public Ready GetReadyState()
        {
            return Ready;
        }

        public Airborne GetAirborneState()
        {
            return Airborne;
        }

        public Grounded GetGroundedState()
        {
            return Grounded;
        }

        private void ToggleCursorLock()
        {
            var isLocked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !isLocked;
        }

        #region fighting states

        private FightingStateMachine _fightingStateMachine;
        public Ready Ready { get; private set; }

        #endregion

        # region movement states

        private MovementStateMachine _movementStateMachine;
        public Grounded Grounded { get; private set; }
        public Airborne Airborne { get; private set; }

        #endregion

        #region New Unity Input System Callbacks

        public void OnLook(InputValue value)
        {
            LookDirection = value.Get<Vector2>();
        }

        public void OnMove(InputValue value)
        {
            MoveDirection = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            JumpRequested = value.isPressed;
        }

        public void OnAttack(InputValue value)
        {
            AttackRequested = value.isPressed;
        }

        #endregion
    }
}