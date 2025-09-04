/// <summary>
/// The abstract base class for all player states. Provides the blueprint for state behavior.
/// </summary>
public abstract class PlayerBaseState
{
    /// <summary>
    /// Called once when the state machine transitions into this state.
    /// Used for setup logic.
    /// </summary>
    /// <param name="player">A reference to the player context.</param>
    public abstract void EnterState(Player player);

    /// <summary>
    /// Called every frame via the player's Update method.
    /// Used for handling input and state transitions.
    /// </summary>
    /// <param name="player">A reference to the player context.</param>
    public abstract void UpdateState(Player player);

    /// <summary>
    /// Called every physics step via the player's FixedUpdate method.
    /// Used for applying forces and physics-based movement.
    /// </summary>
    /// <param name="player">A reference to the player context.</param>
    public abstract void FixedUpdateState(Player player);
}
