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
        Vector2 handCheckOrigin = (Vector2)_transform.position + _settings.ledgeCheckOffset;
        Vector2 headCheckOrigin = (Vector2)_transform.position + _settings.headCheckOffset;

        // 1. Hand Check
        RaycastHit2D wallHit = Physics2D.Raycast(handCheckOrigin, Vector2.right * _movement.FacingDirection, _settings.ledgeWallCheckDistance, _settings.groundLayer);
        if (!wallHit.collider)
        {
            // This will only print when the check is active and fails.
            // To avoid spam, we'll only log if the player is falling.
            if (GetComponent<Rigidbody2D>().velocity.y < 0)
                Debug.Log("Ledge Check Failed: Hand check (cyan line) did not hit a wall.");
            return false;
        }

        // 2. Head Check
        RaycastHit2D headClearanceHit = Physics2D.Raycast(headCheckOrigin, Vector2.right * _movement.FacingDirection, _settings.ledgeWallCheckDistance, _settings.groundLayer);
        if (headClearanceHit.collider != null)
        {
            Debug.Log("Ledge Check Failed: Head check (red line) was obstructed.");
            return false;
        }

        // 3. Surface Check
        Vector2 ledgeSurfaceOrigin = new Vector2(wallHit.point.x + (_settings.ledgeSurfaceCheckOffset.x * _movement.FacingDirection), wallHit.point.y + _settings.ledgeSurfaceCheckOffset.y);
        RaycastHit2D surfaceHit = Physics2D.Raycast(ledgeSurfaceOrigin, Vector2.down, _settings.ledgeSurfaceCheckDistance, _settings.groundLayer);
        if (!surfaceHit.collider)
        {
            Debug.Log("Ledge Check Failed: Could not find a surface on top of the ledge.");
            return false;
        }

        // 4. Stand Position Check
        Vector2 standPositionCheckOrigin = surfaceHit.point + _settings.ledgeStandPositionOffset;
        Collider2D obstacle = Physics2D.OverlapBox(standPositionCheckOrigin, _settings.ledgeStandCheckSize, 0f, _settings.groundLayer);
        if (obstacle)
        {
            Debug.Log("Ledge Check Failed: The space to stand on the ledge was obstructed.");
            return false;
        }

        // If we get here, all checks passed!
        Debug.Log("Ledge Detected Successfully!");
        LedgeStandPosition = new Vector2(surfaceHit.point.x + (_settings.ledgeStandPositionOffset.x * _movement.FacingDirection), surfaceHit.point.y);
        return true;
    }

    /// <summary>
    /// Draws gizmos in the editor to visualize the ledge detection raycasts and boxes.
    /// </summary>
    public void DrawGizmos()
    {
        if (_settings == null || _movement == null) return;

        Vector2 handCheckOrigin = (Vector2)transform.position + _settings.ledgeCheckOffset;
        Vector2 headCheckOrigin = (Vector2)transform.position + _settings.headCheckOffset;


        // Visualize Wall Check
        Vector2 raycastOrigin = (Vector2)transform.position + _settings.ledgeCheckOffset;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(handCheckOrigin, handCheckOrigin + (Vector2.right * _movement.FacingDirection * _settings.ledgeWallCheckDistance));

        // You can add more gizmos here to visualize the other checks (surface hit, stand position)
        // This is highly recommended for easier debugging and level design.
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(LedgeStandPosition, _settings.ledgeStandCheckSize);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(headCheckOrigin, headCheckOrigin + (Vector2.right * _movement.FacingDirection * _settings.ledgeWallCheckDistance));

    }
}
