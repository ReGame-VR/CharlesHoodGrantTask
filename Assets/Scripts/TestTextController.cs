using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTextController : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
        text.text = "max front:" + GlobalControl.Instance.forwardCal
            + "\nmax back: " + GlobalControl.Instance.backwardsCal
            + " \nmaxLeft: " + GlobalControl.Instance.leftCal
            + " \nmaxRight: \n" + GlobalControl.Instance.rightCal; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
