using UnityEngine;

/// <summary>
/// Stores calibration data for trial use in a single place.
/// </summary>
public class GlobalControl : MonoBehaviour {
    // The calibration data collected in the calibration scene and Calibrate.cs
    public float forwardCal, backwardsCal, leftCal, rightCal, armLength, shoulderHeight, 
        maxLeftReach, maxRightReach/*, backReach, frontReach*/;

    // todo: set this in menu
    public bool isRotation = true;

    // The single instance of this class
    public static GlobalControl Instance;

    /// <summary>
    /// Assign instance to this, or destroy it if Instance already exits and is not this instance.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
