using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task : MonoBehaviour {
    [SerializeField]
    private GameObject controller1, controller2;

    private bool straightOutCal, crossedOutCal;

    private Vector2 


	// Use this for initialization
	void Start () {
        straightOutCal = false;
        crossedOutCal = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (!straightOutCal)
        {

        }
        else if (!crossedOutCal)
        {
            // if key pressed
            if 
        }
        else
        {

        }
	}

    /// <summary>
    /// If the user is way outside of the position, it will be red.
    /// If the user is close, it will be yellow
    /// If the useris inside, it will be green
    /// </summary>
    private enum posnIndicator { RED, YELLOW, GREEN };
}
