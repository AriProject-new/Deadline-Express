using UnityEngine;

/// <summary>
/// The state for when the player is airborne (jumping or falling).
/// </summary>
public class PlayerInAirState : PlayerBaseState
{
    public override void EnterState(Player player) { }

    public override void UpdateState(Player player)
    {
        // Check for double jump input
        if (player.JumpBufferCounter > 0 && player.Jump.JumpsLeft > 0)
        {
            player.PerformAirJump();
        }

        // Transition to other states
        if (player.IsGroundedCheck())
        {
            player.ChangeState(player.IdleState);
        }
        else if (player.WallInteractor.IsTouchingWall(player.WallCheckTransform) && player.InputManager.HorizontalInput * player.Movement.FacingDirection > 0)
        {
            player.ChangeState(player.WallSlidingState);
        }
    }

    public override void FixedUpdateState(Player player)
    {
        // Apply horizontal movement and modified gravity.
        player.Movement.HandleMovement(player.Rb, player.InputManager.HorizontalInput, player.InputManager.IsRunning, false, true, player.Settings);
        player.Jump.HandleGravity(player.Rb, player.Settings);
    }
}
