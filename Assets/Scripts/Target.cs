using UnityEngine;

/// <summary>
/// A class to represent each of 6 targets - each target has a position, target center of pressure,
/// and a color that indicates how close the user is to the center of pressure
/// </summary>
public class Target {
    public posnIndicator indication;
    public readonly Vector3 worldPosn;
    public readonly Vector2 CoPTarget;

    public Target(Vector3 worldPosn, Vector2 CoPTarget)
    {
        this.indication = posnIndicator.RED;
        this.worldPosn = worldPosn;
        this.CoPTarget = CoPTarget;
    }

    /// <summary>
    /// If the user is way outside of the position, it will be red.
    /// If the user is close, it will be yellow
    /// If the useris inside, it will be green
    /// </summary>
    public enum posnIndicator { RED, YELLOW, GREEN };
}
