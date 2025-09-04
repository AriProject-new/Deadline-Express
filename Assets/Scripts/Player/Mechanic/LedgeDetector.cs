using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specialized component responsible for detecting ledges that the player can climb.
/// It uses two raycasts: one to find a wall in front of the player, and one to check for empty space above the wall.
/// </summary>
public class LedgeDetector : MonoBehaviour
{
    private PlayerMovementSettings _settings;
    private PlayerMovement _movement;
    private Transform _transform;

    /// <summary>
    /// The final, calculated position where the player should be after completing the ledge climb.
    /// </summary>
    public Vector2 LedgeStandPosition { get; private set; }

    public void Initialize(Player player)
    {
        _settings = player.Settings;
        _movement = player.Movement;
        _transform = transform;
    }

    /// <summary>
    /// Checks for a climbable ledge.
    /// </summary>
    /// <returns>True if a valid ledge is detected, otherwise false.</returns>
    public bool DetectLedge()
    {
        // The origin point for our raycasts, slightly offset from the player's pivot.
        Vector2 raycastOrigin = (Vector2)_transform.position + _settings.ledgeCheckOffset;

        // Cast forward to detect the wall of the ledge.
        RaycastHit2D wallHit = Physics2D.Raycast(raycastOrigin, Vector2.right * _movement.FacingDirection, _settings.ledgeWallCheckDistance, _settings.groundLayer);

        if (!wallHit.collider)
        {
            // No wall detected in front.
            return false;
        }

        // Cast downward from above the wall hit to find the top surface of the ledge.
        Vector2 ledgeSurfaceOrigin = new Vector2(wallHit.point.x + (_settings.ledgeSurfaceCheckOffset.x * _movement.FacingDirection), wallHit.point.y + _settings.ledgeSurfaceCheckOffset.y);
        RaycastHit2D surfaceHit = Physics2D.Raycast(ledgeSurfaceOrigin, Vector2.down, _settings.ledgeSurfaceCheckDistance, _settings.groundLayer);

        if (!surfaceHit.collider)
        {
            // No ground surface found for the ledge top.
            return false;
        }

        // Ensure there is clear space for the player to stand on the ledge.
        Vector2 standPositionCheckOrigin = surfaceHit.point + _settings.ledgeStandPositionOffset;
        Collider2D obstacle = Physics2D.OverlapBox(standPositionCheckOrigin, _settings.ledgeStandCheckSize, 0f, _settings.groundLayer);

        if (obstacle)
        {
            // The space where the player would stand is obstructed.
            return false;
        }

        // A valid ledge has been found. Calculate the position to snap the player to.
        LedgeStandPosition = surfaceHit.point + new Vector2(0f, _settings.ledgeStandPositionOffset.y - _settings.ledgeStandCheckSize.y / 2f);
        return true;
    }

    /// <summary>
    /// Draws gizmos in the editor to visualize the ledge detection raycasts and boxes.
    /// </summary>
    public void DrawGizmos()
    {
        if (_settings == null || _movement == null) return;

        // Visualize Wall Check
        Vector2 raycastOrigin = (Vector2)transform.position + _settings.ledgeCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(raycastOrigin, raycastOrigin + (Vector2.right * _movement.FacingDirection * _settings.ledgeWallCheckDistance));

        // You can add more gizmos here to visualize the other checks (surface hit, stand position)
        // This is highly recommended for easier debugging and level design.
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(LedgeStandPosition, _settings.ledgeStandCheckSize);
    }
}
