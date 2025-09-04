using UnityEngine;
/// <summary>
/// Toolkit spesialis untuk semua logika gerakan horizontal.
/// Menerima perintah dari Player.cs.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public int FacingDirection { get; private set; }

    private void Awake()
    {
        FacingDirection = 1;
    }

    public void HandleMovement(Rigidbody2D rb, float horizontalInput, bool isRunning, bool isGrounded, bool canFlip, PlayerMovementSettings settings)
    {
        HandleFlip(horizontalInput, canFlip);
        
        float targetSpeed = horizontalInput * (isRunning ? settings.maxSpeedRun : settings.maxSpeedWalk);
        float accel = isGrounded ? (Mathf.Abs(horizontalInput) > 0.1f ? settings.acceleration : settings.decceleration) : settings.airAcceleration;
        
        float newHorizontalVelocity = Mathf.MoveTowards(rb.velocity.x, targetSpeed, accel * Time.fixedDeltaTime);
        
        rb.velocity = new Vector2(newHorizontalVelocity, rb.velocity.y);
    }

    private void HandleFlip(float horizontalInput, bool canFlip)
    {
        if (!canFlip) return;

        if (horizontalInput > 0 && FacingDirection == -1)
        {
            Flip();
        }
        else if (horizontalInput < 0 && FacingDirection == 1)
        {
            Flip();
        }
    }

    public void Flip()
    {
        FacingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
