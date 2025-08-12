using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementSettings", menuName = "Player/Movement Settings")]
public class PlayerMovementSettings : ScriptableObject
{
    [Header("Walking & Running")]
    public float maxSpeedWalk = 5f;
    public float maxSpeedRun = 8f;
    public float acceleration = 50f;
    public float decceleration = 100f;
    public float airAcceleration = 25f;

    [Header("Jumping & Gravity")]
    public float jumpForce = 18f;
    public int maxJumps = 2;

    [Header("Game Feel (Wajib Diatur!)")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.15f;
    public float doubleJumpCooldown = 0.2f;
    public float wallJumpLockoutTime = 0.15f;
    public float wallDetachFlipLockoutTime = 0.1f;

    [Header("Apex & Gravity Modifiers")]
    public float fallGravityMultiplier = 1.5f;
    public float apexGravityMultiplier = 0.8f;
    public float apexHeightThreshold = 2f;
    
    [Header("Wall Mechanics")]
    public LayerMask wallLayer;
    public float wallCheckDistance = 0.5f;
    public float wallSlideSpeed = 2f;
    public Vector2 wallJumpForce = new Vector2(15f, 20f);
    
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 0.1f;
    public Vector2 groundCheckSize = new Vector2(0.8f, 0.2f);
}
