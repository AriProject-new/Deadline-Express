using UnityEngine;
public enum PlayerStateEnum { Idle, Moving, InAir, WallSliding, WallJumping }

[RequireComponent(typeof(WallInteractor), typeof(PlayerAnimator))] 
public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementSettings Settings { get; private set; }
    [SerializeField] private PlayerStateEnum currentState;

    #region Komponen & Referensi
    public PlayerAnimator PlayerAnim { get; private set; }
    public Rigidbody2D Rb { get; private set; }
    public InputManager InputManager { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerJump Jump { get; private set; }
    public WallInteractor WallInteractor { get; private set; }
    #endregion

    #region Variabel Pengecekan & Timer
    [Header("Debug Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform wallCheck;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float wallJumpLockoutTimer;
    private float flipLockoutTimer;
    #endregion

    private void Awake()
    {
        PlayerAnim = GetComponent<PlayerAnimator>();
        Rb = GetComponent<Rigidbody2D>();
        InputManager = GetComponent<InputManager>();
        Movement = GetComponent<PlayerMovement>();
        Jump = GetComponent<PlayerJump>();
        WallInteractor = GetComponent<WallInteractor>();
    }

    private void Start()
    {
        Jump.Initialize(Settings);
        WallInteractor.Initialize(this);
        ChangeState(PlayerStateEnum.Idle);
    }

    private void Update()
    {
        if (wallJumpLockoutTimer > 0) wallJumpLockoutTimer -= Time.deltaTime;
        if (flipLockoutTimer > 0) flipLockoutTimer -= Time.deltaTime;

        UpdateTimersAndChecks();
        HandleStateTransitions();
        PlayerAnim.UpdateAnimationParameters(Rb, isGrounded, currentState == PlayerStateEnum.WallSliding, Jump.JumpsLeft);
    }

    private void FixedUpdate()
    {
        HandleStatePhysics();
    }

    #region FSM Core Logic
    private void HandleStateTransitions()
    {
        switch (currentState)
        {
            case PlayerStateEnum.Idle:
                if (InputManager.HorizontalInput != 0) ChangeState(PlayerStateEnum.Moving);
                else if (!isGrounded) ChangeState(PlayerStateEnum.InAir);
                if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) PerformGroundJump();
                break;
            case PlayerStateEnum.Moving:
                if (InputManager.HorizontalInput == 0) ChangeState(PlayerStateEnum.Idle);
                else if (!isGrounded) ChangeState(PlayerStateEnum.InAir);
                if (jumpBufferCounter > 0 && coyoteTimeCounter > 0) PerformGroundJump();
                break;
            case PlayerStateEnum.InAir:
                if (isGrounded) ChangeState(PlayerStateEnum.Idle);
                else if (WallInteractor.IsTouchingWall(wallCheck) && InputManager.HorizontalInput * Movement.FacingDirection > 0)
                    ChangeState(PlayerStateEnum.WallSliding);
                if (jumpBufferCounter > 0 && Jump.JumpsLeft > 0) PerformAirJump();
                break;
            case PlayerStateEnum.WallSliding:
                if (jumpBufferCounter > 0) ChangeState(PlayerStateEnum.WallJumping);
                else if (isGrounded) ChangeState(PlayerStateEnum.Idle);
                else if (!WallInteractor.IsTouchingWall(wallCheck) || InputManager.HorizontalInput * Movement.FacingDirection < 0)
                {
                    Jump.DisableAirJumps();
                    flipLockoutTimer = Settings.wallDetachFlipLockoutTime;
                    ChangeState(PlayerStateEnum.InAir);
                }
                break;
            case PlayerStateEnum.WallJumping:
                if (wallJumpLockoutTimer <= 0)
                {
                    ChangeState(PlayerStateEnum.InAir);
                }
                break;
        }
    }

    private void HandleStatePhysics()
    {
        bool canFlip = flipLockoutTimer <= 0 && wallJumpLockoutTimer <= 0;
        switch (currentState)
        {
            case PlayerStateEnum.Idle:
                Movement.HandleMovement(Rb, 0, false, true, canFlip, Settings);
                break;
            case PlayerStateEnum.Moving:
                Movement.HandleMovement(Rb, InputManager.HorizontalInput, InputManager.IsRunning, true, canFlip, Settings);
                break;
            case PlayerStateEnum.InAir:
                Movement.HandleMovement(Rb, InputManager.HorizontalInput, InputManager.IsRunning, false, canFlip, Settings);
                Jump.HandleGravity(Rb, Settings);
                break;
            case PlayerStateEnum.WallSliding:
                WallInteractor.HandleWallSlide();
                break;
            case PlayerStateEnum.WallJumping:
                Jump.HandleGravity(Rb, Settings);
                break;
        }
    }
    #endregion
    
    #region Aksi & Helper
    private void PerformGroundJump()
    {
        PlayerAnim.TriggerJump();
        if (Jump.PerformJump(Rb, Settings))
        {
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            ChangeState(PlayerStateEnum.InAir);
        }
    }

    private void PerformAirJump()
    {
        PlayerAnim.TriggerJump();
        if (Jump.PerformJump(Rb, Settings)) jumpBufferCounter = 0;
    }
    
    private void UpdateTimersAndChecks()
    {
        isGrounded = IsGroundedCheck();
        bool justLanded = isGrounded && !wasGroundedLastFrame;
        if (justLanded) { Jump.ResetJumpsOnGround(Settings); WallInteractor.ResetWallJumpMemory(); }
        if (isGrounded) coyoteTimeCounter = Settings.coyoteTime; else coyoteTimeCounter -= Time.deltaTime;
        if (InputManager.JumpPressed) jumpBufferCounter = Settings.jumpBufferTime; else jumpBufferCounter -= Time.deltaTime;
        Jump.UpdateCooldownTimer();
        wasGroundedLastFrame = isGrounded;
    }

    private void ChangeState(PlayerStateEnum newState)
    {
        if (newState == currentState) return;
        currentState = newState;
        
        if (newState == PlayerStateEnum.WallSliding)
        {
            Jump.GrantSingleWallJump();
        }
        else if (newState == PlayerStateEnum.WallJumping)
        {
            if (WallInteractor.PerformWallJump())
            {
                jumpBufferCounter = 0;
                Jump.DisableAirJumps();
                wallJumpLockoutTimer = Settings.wallJumpLockoutTime;
                PlayerAnim.TriggerJump();
            }
        }
    }
    #endregion

    #region Pengecekan Fisika & Gizmos
    public bool IsGroundedCheck() { return Physics2D.BoxCast(groundCheck.position, Settings.groundCheckSize, 0f, Vector2.down, Settings.groundCheckDistance, Settings.groundLayer); }
    private void OnDrawGizmos()
    {
        if (Settings == null || groundCheck == null || wallCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheck.position + (Vector3)(Vector2.down * Settings.groundCheckDistance), Settings.groundCheckSize);
        Gizmos.color = Color.blue;
        if (Movement != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * Movement.FacingDirection * Settings.wallCheckDistance));
    }
    #endregion
}
