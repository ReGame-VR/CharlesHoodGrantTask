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

    /// <summary>
    /// Returns the score based on the 2D point IN RELATION TO THE CENTER OF THE TARGET
    /// and the time it was hit. Must translate from Unity units (meters) to pixels due
    /// to physical environment.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="currTime"></param>
    /// <returns></returns>
    public static float ScoreTouch(Vector2 point, float currTime)
    {
        float pixPerM = 6489.2f;

        float speed = 10 - currTime;

        float accuracy = (100 - Mathf.Abs(point.x * pixPerM - 500)/10 
            + Mathf.Abs(point.y * pixPerM -500) / 10)*0.9f;

        return speed + accuracy;
    }
}
