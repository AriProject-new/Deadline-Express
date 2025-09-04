using UnityEngine;

/// <summary>
/// The main controller for the player character. This class acts as the "brain" or "context" for the State Machine.
/// It holds all the component references and core variables, but delegates the frame-by-frame logic to the current state object.
/// </summary>
[RequireComponent(typeof(WallInteractor))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(LedgeDetector))] // Added LedgeDetector
public class Player : MonoBehaviour
{
    #region State Machine
    /// <summary>
    /// The current active state in the state machine.
    /// </summary>
    public PlayerBaseState CurrentState { get; private set; }

    // State Instances
    public readonly PlayerIdleState IdleState = new PlayerIdleState();
    public readonly PlayerMovingState MovingState = new PlayerMovingState();
    public readonly PlayerInAirState InAirState = new PlayerInAirState();
    public readonly PlayerWallSlidingState WallSlidingState = new PlayerWallSlidingState();
    public readonly PlayerWallJumpingState WallJumpingState = new PlayerWallJumpingState();
    public readonly PlayerLedgeClimbingState LedgeClimbingState = new PlayerLedgeClimbingState(); // New State
    #endregion

    #region Component References
    /// <summary>
    /// The scriptable object containing all movement tuning values.
    /// </summary>
    [field: SerializeField] public PlayerMovementSettings Settings { get; private set; }

    // Core Components
    public PlayerAnimator PlayerAnim { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public InputManager InputManager { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerJump Jump { get; private set; }
    public WallInteractor WallInteractor { get; private set; }
    public LedgeDetector LedgeDetector { get; private set; } // New Component

    // Check Transforms
    [field: Header("Checks")]
    [field: SerializeField] public Transform GroundCheckTransform { get; private set; }
    [field: SerializeField] public Transform WallCheckTransform { get; private set; }
    #endregion

    #region Core Properties & Timers
    public float CoyoteTimeCounter { get; private set; }
    public float JumpBufferCounter { get; private set; }
    public float WallJumpLockoutTimer { get; private set; }
    private bool isMovementLocked = false;
    private bool wasGroundedLastFrame;
    private float flipLockoutTimer;
    private DeliveryPoint currentDeliveryPoint;

    /// <summary>
    /// A flag used by the test suite to verify that UnlockMovement was called.
    /// </summary>
    [System.NonSerialized] public bool UnlockMovementWasCalled = false;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        // Get all component references
        PlayerAnim = GetComponent<PlayerAnimator>();
        Rb = GetComponent<Rigidbody2D>();
        InputManager = GetComponent<InputManager>();
        Movement = GetComponent<PlayerMovement>();
        Jump = GetComponent<PlayerJump>();
        WallInteractor = GetComponent<WallInteractor>();
        LedgeDetector = GetComponent<LedgeDetector>(); // Get LedgeDetector
    }

    private void Start()
    {
        // Initialize sub-components that need it
        Jump.Initialize(Settings);
        WallInteractor.Initialize(this);
        LedgeDetector.Initialize(this); // Initialize LedgeDetector

        // Start in the Idle state
        ChangeState(IdleState);
    }

    private void OnEnable()
    {
        GameEvents.OnTypingSessionStart += LockMovement;
        GameEvents.OnTypingSessionEnd += UnlockMovement;
    }

    private void OnDisable()
    {
        GameEvents.OnTypingSessionStart -= LockMovement;
        GameEvents.OnTypingSessionEnd -= UnlockMovement;
    }

    private void Update()
    {
        if (isMovementLocked) return;

        // --- DELEGATE TO STATE ---
        CurrentState.UpdateState(this);

        // --- TIMERS & CHECKS ---
        UpdateTimersAndChecks();
        if (WallJumpLockoutTimer > 0) WallJumpLockoutTimer -= Time.deltaTime;
        if (flipLockoutTimer > 0) flipLockoutTimer -= Time.deltaTime;

        // --- INTERACTION ---
        if (currentDeliveryPoint != null && InputManager.InteractPressed)
        {
            currentDeliveryPoint.StartInteraction();
        }

        // --- ANIMATION ---
        bool isWallSliding = CurrentState is PlayerWallSlidingState;
        PlayerAnim.UpdateAnimationParameters(Rb, IsGroundedCheck(), isWallSliding, Jump.JumpsLeft);
    }

    private void FixedUpdate()
    {
        if (isMovementLocked) return;

        // Delegate all physics-based logic to the current state
        CurrentState.FixedUpdateState(this);
    }
    #endregion

    #region Public Methods for States
    /// <summary>
    /// Changes the current state of the player FSM.
    /// </summary>
    public void ChangeState(PlayerBaseState newState)
    {
        CurrentState?.ExitState(this); // Call ExitState on the old state
        CurrentState = newState;
        CurrentState.EnterState(this);
    }

    public bool IsGroundedCheck()
    {
        return Physics2D.BoxCast(GroundCheckTransform.position, Settings.groundCheckSize, 0f, Vector2.down, Settings.groundCheckDistance, Settings.groundLayer);
    }

    public void PerformGroundJump()
    {
        if (Jump.PerformJump(Rb, Settings))
        {
            ResetJumpBuffer();
            CoyoteTimeCounter = 0;
            PlayerAnim.TriggerJump();
        }
    }

    public void PerformAirJump()
    {
        if (Jump.PerformJump(Rb, Settings))
        {
            ResetJumpBuffer();
            PlayerAnim.TriggerJump();
        }
    }

    public void ResetJumpBuffer() => JumpBufferCounter = 0;

    public void StartWallJumpLockout() => WallJumpLockoutTimer = Settings.wallJumpLockoutTime;

    public void StartWallDetachLockout()
    {
        flipLockoutTimer = Settings.wallDetachFlipLockoutTime;
        Jump.DisableAirJumps();
    }

    private void UpdateTimersAndChecks()
    {
        // Check if we are currently grounded
        bool isGrounded = IsGroundedCheck();

        // Check if we just landed this frame
        if (isGrounded && !wasGroundedLastFrame)
        {
            Jump.ResetJumpsOnGround(Settings);
            WallInteractor.ResetWallJumpMemory();
        }

        // Manage Coyote Time
        if (isGrounded)
        {
            CoyoteTimeCounter = Settings.coyoteTime;
        }
        else
        {
            CoyoteTimeCounter -= Time.deltaTime;
        }

        // Manage Jump Buffer
        if (InputManager.JumpPressed)
        {
            JumpBufferCounter = Settings.jumpBufferTime;
        }
        else
        {
            JumpBufferCounter -= Time.deltaTime;
        }

        // Update other sub-components that need it
        Jump.UpdateCooldownTimer();

        // Store the grounded state for the next frame
        wasGroundedLastFrame = isGrounded;
    }
    #endregion

    #region Movement Lock
    public void LockMovement()
    {
        isMovementLocked = true;
        // Rb.velocity = Vector2.zero; // Note: We don't zero velocity here anymore because states like LedgeClimb might manage it.
        UnlockMovementWasCalled = false;
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
        UnlockMovementWasCalled = true;
    }
    #endregion

    #region Triggers & Gizmos
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeliveryPoint"))
        {
            currentDeliveryPoint = other.GetComponent<DeliveryPoint>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("DeliveryPoint"))
        {
            currentDeliveryPoint = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (Settings == null || GroundCheckTransform == null || WallCheckTransform == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GroundCheckTransform.position + Vector3.down * Settings.groundCheckDistance, Settings.groundCheckSize);

        Gizmos.color = Color.blue;
        if (Movement != null)
        {
            Gizmos.DrawLine(WallCheckTransform.position, WallCheckTransform.position + Vector3.right * Movement.FacingDirection * Settings.wallCheckDistance);
        }

        // Draw Ledge Detector Gizmos
        if (LedgeDetector != null)
        {
            LedgeDetector.DrawGizmos();
        }
    }
    #endregion
}
