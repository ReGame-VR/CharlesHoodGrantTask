using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {

    /*
        Atar = [XMinWeight/2, YMaxWeight - halfdim]; %Center coordinates of target A
        Btar = [XMaxWeight - halfdim, YMaxWeight - halfdim]; %Center coordinates of target B
        Ctar = [XMinWeight + halfdim, 0]; %Center coordinates of target C
        Dtar = [2 + halfdim, 0]; %Center coordinates of target D
        Etar = [0 - halfdim, YMinWeight + halfdim]; %Center coordinates of target E
        Ftar = [(XMaxWeight - 2) - halfdim, YMinWeight + halfdim]; %Center coordinates of target 
    */


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
