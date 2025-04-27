using MovementState;
using UnityEngine;
using UnityEngine.InputSystem;

// Keep if you use Input System

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

    private float _cameraRotationX;
    private bool _jumpRequested;

    private CharacterStateMachine _stateMachine;

    public CharacterController Controller { get; private set; }
    public Camera PlayerCamera { get; private set; }
    public Vector2 LookDirection { get; private set; } = Vector2.zero;
    public Vector2 MoveDirection { get; private set; } = Vector2.zero;
    public bool JumpRequested { get; private set; }
    public bool AttackRequested { get; set; }
    public float VerticalVelocity { get; set; }
    public bool IsGrounded { get; private set; }
    public GroundedState GroundedState { get; private set; }
    public AirborneState AirborneState { get; private set; }


    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        PlayerCamera = Camera.main;

        if (PlayerCamera == null)
            Debug.LogError("Main Camera not found! Ensure a camera is tagged 'MainCamera'.", this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _stateMachine = new CharacterStateMachine();

        GroundedState = new GroundedState(this, _stateMachine);
        AirborneState = new AirborneState(this, _stateMachine);
    }

    private void Start()
    {
        _stateMachine.Initialize(GroundedState);
    }

    private void Update()
    {
        ProcessLook();

        _stateMachine.CurrentState.HandleInput();
        _stateMachine.CurrentState.LogicUpdate();

        JumpRequested = false;
        AttackRequested = false;
    }

    private void FixedUpdate()
    {
        PerformGroundCheck();

        _stateMachine.CurrentState.PhysicsUpdate();

        _stateMachine.CurrentState.PostPhysicsUpdate();
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

        IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        //RaycastHit hit;
        //IsGrounded = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, groundCheckDistance,
        //    groundMask, QueryTriggerInteraction.Ignore);
        //if (IsGrounded)
        //{
        //    /* Process hit.normal for the slope */
        //}
        //if (!IsGrounded) IsGrounded = Controller.isGrounded;
    }

    private void ToggleCursorLock()
    {
        var isLocked = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !isLocked;
    }

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