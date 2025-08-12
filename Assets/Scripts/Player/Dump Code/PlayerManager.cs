using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] private PlayerMovementSettings settings;

    private Rigidbody2D rb;
    private InputManager inputManager;
    private PlayerMovement playerMovement;
    private PlayerJump playerJump;

    public bool IsGrounded { get; private set; }
    private bool wasGroundedLastFrame;

    // Kita akan menggunakan kedua timer ini
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float landingJumpDelayCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputManager = GetComponent<InputManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerJump = GetComponent<PlayerJump>();
        
        playerJump.Initialize(settings);
    }

    private void Update()
    {
        HandleLandingAndTimers();
        HandleJumpLogic();
        
        wasGroundedLastFrame = IsGrounded;
    }

    private void FixedUpdate()
    {
        playerMovement.HandleMovement(rb, inputManager.HorizontalInput, inputManager.IsRunning, IsGrounded, settings);
        playerJump.HandleGravity(rb, settings);
    }

    private void HandleLandingAndTimers()
    {
        IsGrounded = CheckGrounded();
        
        bool justLanded = IsGrounded && !wasGroundedLastFrame;
        if (justLanded)
        {
            playerJump.ResetJumps(settings);
            landingJumpDelayCounter = settings.landingJumpDelay;
        }

        // Hitung mundur semua timer
        if (landingJumpDelayCounter > 0) landingJumpDelayCounter -= Time.deltaTime;
        if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Atur ulang timer berdasarkan kondisi
        if (IsGrounded) coyoteTimeCounter = settings.coyoteTime;
        if (inputManager.JumpPressed) jumpBufferCounter = settings.jumpBufferTime;
        
        playerJump.UpdateCooldownTimer();
    }
    
    private void HandleJumpLogic()
    {
        // Cek jika pemain ingin melompat (ada buffer) DAN jeda pendaratan sudah selesai.
        if (jumpBufferCounter > 0 && landingJumpDelayCounter <= 0)
        {
            // Jika kita berada dalam coyote time, lakukan lompatan darat.
            if (coyoteTimeCounter > 0)
            {
                if (playerJump.PerformJump(rb, settings))
                {
                    // Habiskan buffer dan coyote time agar tidak memicu lompatan lagi.
                    jumpBufferCounter = 0;
                    coyoteTimeCounter = 0;
                }
            }
        }
        
        // Ini tidak dipengaruhi oleh landing delay, hanya oleh jump buffer.
        if (jumpBufferCounter > 0 && !IsGrounded && playerJump.JumpsLeft > 0)
        {
            // Ini mencegah double jump langsung setelah coyote jump
            if (coyoteTimeCounter <= 0)
            {
                if (playerJump.PerformJump(rb, settings))
                {
                    jumpBufferCounter = 0;
                }
            }
        }
    }

    private bool CheckGrounded()
    {
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(0, -transform.localScale.y * 0.5f);
        return Physics2D.BoxCast(boxOrigin, settings.groundCheckSize, 0f, Vector2.down, settings.groundCheckDistance, settings.groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (settings == null) return;
        Vector2 boxOrigin = (Vector2)transform.position + new Vector2(0, -transform.localScale.y * 0.5f);
        Vector2 boxPosition = boxOrigin + Vector2.down * settings.groundCheckDistance;
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        Gizmos.DrawWireCube(boxPosition, settings.groundCheckSize);
    }
}*/
