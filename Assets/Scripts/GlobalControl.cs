﻿using UnityEngine;

/// <summary>
/// Stores calibration data for trial use in a single place.
/// </summary>
public class GlobalControl : MonoBehaviour {
    // The calibration data collected in the calibration scene and Calibrate.cs
    public float forwardCal, backwardsCal, leftCal, rightCal, armLength, shoulderHeight, 
        maxLeftReach, maxRightReach;

    public int numSequences = 5;

    // if this is true, the targets are rotating
    public bool isRotation = true;

    // if this is true, participant is right handed
    public bool rightHanded = true;

    // participant ID to differentiate data files
    public string participantID;

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
