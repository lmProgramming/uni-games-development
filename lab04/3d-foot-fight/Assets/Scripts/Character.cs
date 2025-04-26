using CharacterState;
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

    [SerializeField] private float lookXLimit = 80.0f; // Limit vertical look angle

    [Header("Ground Check Settings")]
    [Tooltip("Layer Mask for spherecasts. Assign the Player object to a Layer, then Ignore that layer here.")]
    [SerializeField]
    private LayerMask groundMask = ~0; // Default to everything

    [Tooltip("How far below the character's center the ground check starts.")] [SerializeField]
    private float groundCheckYOffset = -0.85f; // Adjust based on CharacterController center/height

    [Tooltip("Radius of the ground check sphere.")] [SerializeField]
    private float groundCheckRadius = 0.4f; // Adjust based on CharacterController radius

    [Tooltip("Distance the spherecast checks downwards.")] [SerializeField]
    private float groundCheckDistance = 0.3f; // Small distance is usually enough
    // Add other states here (e.g., CrouchingState, RunningState)


    // --- Private Variables ---
    private float _cameraRotationX;
    private bool _jumpRequested;

    // --- State Machine ---
    private CharacterStateMachine _stateMachine;

    // --- Public Properties (for States) ---
    public CharacterController Controller { get; private set; }
    public Camera PlayerCamera { get; private set; }
    public Vector2 LookDirection { get; private set; } = Vector2.zero;
    public Vector2 MoveDirection { get; private set; } = Vector2.zero;
    public bool JumpRequested { get; private set; }
    public float VerticalVelocity { get; set; } // Public setter for states
    public bool IsGrounded { get; private set; } // Public getter for states
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
        // Handle Input Reading (if using standard input or need frame-based checks)
        // Note: OnLook/OnMove handle Input System callbacks directly

        // Process Look Rotation (usually smoother in Update)
        ProcessLook();

        // Delegate input handling and logic updates to the current state
        _stateMachine.CurrentState.HandleInput();
        _stateMachine.CurrentState.LogicUpdate();

        // Example: Toggle cursor lock (like BasicFPCC)
        if (Input.GetKeyDown(KeyCode.BackQuote)) // Use your Input System action if preferred
            ToggleCursorLock();
    }

    private void FixedUpdate()
    {
        // Perform the ground check BEFORE state physics update
        PerformGroundCheck();

        // Delegate physics updates to the current state
        _stateMachine.CurrentState.PhysicsUpdate();

        // Perform post-physics checks (like checking ground AFTER move)
        _stateMachine.CurrentState.PostPhysicsUpdate();
    }

    // --- Gizmos (Optional, for debugging ground check) ---
    private void OnDrawGizmosSelected()
    {
        var transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        var transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (IsGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // Draw the ground check sphere
        var spherePosition = new Vector3(transform.position.x, transform.position.y + groundCheckYOffset,
            transform.position.z);
        Gizmos.DrawSphere(spherePosition, groundCheckRadius);

        // If using SphereCast, draw the cast distance too
        // Gizmos.DrawWireSphere(spherePosition + Vector3.down * groundCheckDistance, groundCheckRadius);
        // Gizmos.DrawLine(spherePosition, spherePosition + Vector3.down * groundCheckDistance);
    }

    private void ProcessLook()
    {
        if (PlayerCamera == null) return;

        // Get look input scaled by speed and time
        // Note: Input System's 'Delta' mode for mouse might not need Time.deltaTime scaling here
        // Adjust sensitivity as needed
        var lookX = LookDirection.x * lookSpeed * Time.deltaTime * 50f; // Added arbitrary multiplier, adjust lookSpeed
        var lookY = LookDirection.y * lookSpeed * Time.deltaTime * 50f;

        // Rotate Player horizontally (Y-axis)
        transform.Rotate(Vector3.up * lookX);

        // Rotate Camera vertically (X-axis)
        _cameraRotationX -= lookY; // Subtract because rotating up/down is inverse
        _cameraRotationX = Mathf.Clamp(_cameraRotationX, -lookXLimit, lookXLimit);

        PlayerCamera.transform.localRotation = Quaternion.Euler(_cameraRotationX, 0f, 0f);
    }

    private void PerformGroundCheck()
    {
        // Use the CharacterController's center and height/radius for more robustness if needed,
        // but simple offsets often work fine.
        var spherePosition = new Vector3(transform.position.x, transform.position.y + groundCheckYOffset,
            transform.position.z);

        // Perform the sphere cast
        IsGrounded = Physics.CheckSphere(spherePosition, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);

        // Optional: Use SphereCast for more detailed info like slope angle (like BasicFPCC)
        // RaycastHit hit;
        // IsGrounded = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        // if (IsGrounded) { /* Process hit.normal for slope */ }


        // Also consider the CharacterController's built-in check, especially after moving
        // Useful redundancy, especially if CheckSphere misses slightly due to offsets
        // if (!IsGrounded) {
        //     IsGrounded = Controller.isGrounded;
        // }
    }

    // --- Input System Callbacks ---
    // (Keep these as they are if using Input System PlayerInput component)
    public void OnLook(InputValue value) // Change parameter name if needed
    {
        LookDirection = value.Get<Vector2>();
    }

    public void OnMove(InputValue value) // Change parameter name if needed
    {
        MoveDirection = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (!value.isPressed) return;

        JumpRequested = true;
    }


    // --- Utility ---
    private void ToggleCursorLock()
    {
        var isLocked = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = isLocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !isLocked;
    }
}