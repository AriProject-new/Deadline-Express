/// <summary>
/// The brief, transitional state after a player performs a wall jump.
/// </summary>
public class PlayerWallJumpingState : PlayerBaseState
{
    public override void EnterState(Player player)
    {
        // Attempt to perform the wall jump.
        if (player.WallInteractor.PerformWallJump())
        {
            // If successful, set up the state.
            player.ResetJumpBuffer();
            player.Jump.DisableAirJumps();
            player.StartWallJumpLockout();
            player.PlayerAnim.TriggerJump();
        }
        else
        {
            // If the jump fails (e.g., same wall), immediately transition to InAir to prevent getting stuck.
            player.ChangeState(player.InAirState);
        }
    }

    public override void UpdateState(Player player)
    {
        // After the lockout timer finishes, return control to the player in the air.
        if (player.WallJumpLockoutTimer <= 0)
        {
            player.ChangeState(player.InAirState);
        }
    }

    public override void FixedUpdateState(Player player)
    {
        // Continue to apply gravity during the wall jump arc.
        player.Jump.HandleGravity(player.Rb, player.Settings);
    }
}
