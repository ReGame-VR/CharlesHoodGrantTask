using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalControl : MonoBehaviour {
    public float forwardCal, backwardsCal, leftCal, rightCal, armLength, shoulderHeight, maxLeftReach, maxRightReach/*, backReach, frontReach*/;

    public static GlobalControl Instance;

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

        ControllerHandler.NewMaxLeft += setMaxLeft;
        ControllerHandler.NewMaxRight += setMaxRight;
    }

    void OnDisable()
    {
        ControllerHandler.NewMaxLeft -= setMaxLeft;
        ControllerHandler.NewMaxRight -= setMaxRight;
    }

    private void setMaxLeft(float max)
    {
        maxLeftReach = max;
    }

    private void setMaxRight(float max)
    {
        maxRightReach = max;
    }
}
