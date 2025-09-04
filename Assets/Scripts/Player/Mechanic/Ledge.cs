using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple component to mark a GameObject as a grabbable ledge.
/// This provides a precise point for the player to snap to, giving designers pixel-perfect control.
/// </summary>
public class Ledge : MonoBehaviour
{
    /// <summary>
    /// The exact position where the player's hands should snap to when grabbing the ledge.
    /// The player will then be moved on top of this point after the climb.
    /// </summary>
    [Tooltip("The exact point where the player should grab and then stand on top of.")]
    [SerializeField] private Transform _climbUpPosition;

    public Vector2 GetClimbUpPosition()
    {
        return _climbUpPosition.position;
    }
}
