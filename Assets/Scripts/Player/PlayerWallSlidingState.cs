/// <summary>
/// The state for when the player is sliding down a wall.
/// </summary>
public class PlayerWallSlidingState : PlayerBaseState
{
    public override void EnterState(Player player)
    {
        // Grant the player a wall jump charge upon entering this state.
        player.Jump.GrantSingleWallJump();
    }

    public override void UpdateState(Player player)
    {
        // Check for wall jump input
        if (player.JumpBufferCounter > 0)
        {
            player.ChangeState(player.WallJumpingState);
            return; // Exit early
        }

        // Check for other transitions
        if (player.IsGroundedCheck())
        {
            player.ChangeState(player.IdleState);
        }
        else if (!player.WallInteractor.IsTouchingWall(player.WallCheckTransform) || player.InputManager.HorizontalInput * player.Movement.FacingDirection <= 0)
        {
            // Detach if player moves away from the wall or stops holding towards it
            player.StartWallDetachLockout();
            player.ChangeState(player.InAirState);
        }
    }

    public override void FixedUpdateState(Player player)
    {
        // Apply the wall slide physics.
        player.WallInteractor.HandleWallSlide();
    }
}
