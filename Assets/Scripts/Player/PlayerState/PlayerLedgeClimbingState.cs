using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The state for when the player has detected and is climbing a ledge.
/// This is a transitional state that locks movement, repositions the player, and then returns to a normal state.
/// </summary>
public class PlayerLedgeClimbingState : PlayerBaseState
{
    private float _climbTimer;
    private Vector2 _startPosition;
    private Vector2 _endPosition;

    public override void EnterState(Player player)
    {
        // Lock physics and movement
        player.Rb.isKinematic = true;
        player.Rb.velocity = Vector2.zero;
        player.LockMovement();

        // Store start/end positions for the climb lerp
        _startPosition = player.transform.position;
        _endPosition = player.LedgeDetector.LedgeStandPosition;
        _climbTimer = 0f;

        // Trigger animation
        player.PlayerAnim.TriggerLedgeClimb();
    }

    public override void UpdateState(Player player)
    {
        _climbTimer += Time.deltaTime;

        // Interpolate player position for a smooth climb animation
        float climbPercentage = _climbTimer / player.Settings.ledgeClimbDuration;
        player.transform.position = Vector2.Lerp(_startPosition, _endPosition, climbPercentage);

        // Check if the climb is finished
        if (_climbTimer >= player.Settings.ledgeClimbDuration)
        {
            player.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdateState(Player player)
    {
        // No physics updates during this state
    }

    public override void ExitState(Player player)
    {
        // Restore physics and movement control
        player.Rb.isKinematic = false;
        player.UnlockMovement();
    }
}
