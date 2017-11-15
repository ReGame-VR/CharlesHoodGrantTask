using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerHandler : MonoBehaviour {

    public delegate void MaxLeft(float max);
    public static MaxLeft NewMaxLeft;

    public delegate void MaxRight(float max);
    public static MaxRight NewMaxRight;

    private float leftMax, rightMax;

    void Awake()
    {
        leftMax = 0;
        rightMax = 0;
    }

    void Update()
    {
        Vector3 posn = gameObject.transform.position;

        // side to side calibration
        if (posn.x > rightMax)
        {
            rightMax = posn.x;
            if (NewMaxRight != null)
            {
                NewMaxRight(rightMax);
            }
        }
        else if (posn.x < leftMax)
        {
            leftMax = posn.x;
            if (NewMaxLeft != null)
            {
                NewMaxLeft(rightMax);
            }
        }
    }
}
