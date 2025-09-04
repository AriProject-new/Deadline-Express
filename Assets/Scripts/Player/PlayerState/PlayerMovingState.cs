/// <summary>
/// The state for when the player is on the ground and moving horizontally.
/// </summary>
public class PlayerMovingState : PlayerBaseState
{
    public override void EnterState(Player player) { }

    public override void UpdateState(Player player)
    {
        // Check for jump input
        if (player.JumpBufferCounter > 0 && player.CoyoteTimeCounter > 0)
        {
            player.PerformGroundJump();
            player.ChangeState(player.InAirState); // Transition to InAir after jumping
            return; // Exit early to prevent other transitions this frame
        }

        // Transition to other states
        if (player.InputManager.HorizontalInput == 0)
        {
            player.ChangeState(player.IdleState);
        }
        else if (!player.IsGroundedCheck())
        {
            player.ChangeState(player.InAirState);
        }
    }

    public override void FixedUpdateState(Player player)
    {
        // Pass the player's horizontal input to the movement handler.
        player.Movement.HandleMovement(player.Rb, player.InputManager.HorizontalInput, player.InputManager.IsRunning, true, true, player.Settings);
    }
}
