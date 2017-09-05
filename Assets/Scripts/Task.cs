using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {

    [SerializeField]
    private GameObject controller;

    [SerializeField]
    private Image cob;


    private ControllerHandler ctrlr;

	// Use this for initialization
	void Start () {
        ctrlr = (ControllerHandler)controller.GetComponent(typeof(ControllerHandler));
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 posn = Wii.GetCenterOfBalance(0);
	}

    /// <summary>
    /// If the user is way outside of the position, it will be red.
    /// If the user is close, it will be yellow
    /// If the useris inside, it will be green
    /// </summary>
    private enum posnIndicator { RED, YELLOW, GREEN };
}
