using UnityEngine;

    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] public int jumpsLeft;
    [SerializeField] private float doubleJumpCooldownTimer;

    public int JumpsLeft => jumpsLeft;

    public void Initialize(PlayerMovementSettings settings)
    {
        jumpsLeft = settings.maxJumps;
    }
    
    public void UpdateCooldownTimer()
    {
        if (doubleJumpCooldownTimer > 0)
        {
            doubleJumpCooldownTimer -= Time.deltaTime;
        }
    }

    public void ResetJumpsOnGround(PlayerMovementSettings settings)
    {
        jumpsLeft = settings.maxJumps;
        doubleJumpCooldownTimer = 0;
    }

    // --- FUNGSI DIUBAH DAN DITAMBAH ---
    // Mengganti nama agar lebih jelas
    public void DisableAirJumps()
    {
        jumpsLeft = 0;
    }

    // Fungsi baru untuk memberikan satu kesempatan wall jump
    public void GrantSingleWallJump()
    {
        jumpsLeft = 1;
    }
    // ------------------------------------

    public bool PerformJump(Rigidbody2D rb, PlayerMovementSettings settings)
    {
        bool isDoubleJumpAttempt = jumpsLeft < settings.maxJumps;
        
        if (isDoubleJumpAttempt && doubleJumpCooldownTimer > 0)
        {
            return false;
        }
        
        if (jumpsLeft > 0)
        {
            jumpsLeft--;
            
            if (jumpsLeft == settings.maxJumps - 1)
            {
                doubleJumpCooldownTimer = settings.doubleJumpCooldown;
            }
            
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * settings.jumpForce, ForceMode2D.Impulse);
            return true;
        }

        return false;
    }

    public void HandleGravity(Rigidbody2D rb, PlayerMovementSettings settings)
    {
        if (Mathf.Abs(rb.velocity.y) < settings.apexHeightThreshold)
        {
            rb.gravityScale = settings.apexGravityMultiplier;
        }
        else if (rb.velocity.y < 0)
        {
            rb.gravityScale = settings.fallGravityMultiplier;
        }
        else
        {
            rb.gravityScale = 3f;
        }
    }
}

