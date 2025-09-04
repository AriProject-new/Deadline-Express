using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallInteractor : MonoBehaviour
{
    private PlayerMovementSettings settings;
    private Rigidbody2D rb;
    private Transform wallCheck;
    private PlayerMovement movement;

    // Variabel untuk "mengingat"
    private GameObject lastWallJumpedFrom;
    private GameObject currentWallTouching; // Variabel baru untuk melacak dinding saat ini

    public void Initialize(Player player)
    {
        this.settings = player.Settings;
        this.rb = player.Rb;
        this.movement = player.Movement;
    }
    
    // Metode ini sekarang juga bertugas memperbarui dinding yang sedang disentuh
    public bool IsTouchingWall(Transform wallCheckTransform)
    {
        this.wallCheck = wallCheckTransform;
        if (wallCheck == null)
        {
            currentWallTouching = null;
            return false;
        }

        RaycastHit2D hit = Physics2D.Raycast(wallCheck.position, Vector2.right * movement.FacingDirection, settings.wallCheckDistance, settings.wallLayer);

        if (hit.collider != null)
        {
            // Perbarui dinding yang sedang disentuh
            currentWallTouching = hit.collider.gameObject;
            return true;
        }

        // Jika tidak menyentuh apa-apa, reset
        currentWallTouching = null;
        return false;
    }

    public void HandleWallSlide()
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -settings.wallSlideSpeed));
    }
    
    public bool PerformWallJump()
    {
        // GAGAL jika:
        // 1. Tidak ada dinding yang sedang disentuh.
        // 2. Dinding yang disentuh SAMA DENGAN dinding yang terakhir dilompati.
        if (currentWallTouching == null || currentWallTouching == lastWallJumpedFrom)
        {
            return false;
        }

        // Jika berhasil, lakukan lompatan dan "ingat" dinding ini sebagai yang terakhir dilompati
        lastWallJumpedFrom = currentWallTouching;
        rb.velocity = new Vector2(settings.wallJumpForce.x * -movement.FacingDirection, settings.wallJumpForce.y);
        movement.Flip();
        return true;
    }

    // Metode untuk "melupakan" dinding terakhir saat mendarat di tanah
    public void ResetWallJumpMemory()
    {
        lastWallJumpedFrom = null;
    }
}
