using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using 

public class TestTextController : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
        text.text = "max front:" + GlobalControl.Instance.forwardCal * 23.8 / 2
            + " cm\nmax back: " + GlobalControl.Instance.backwardsCal * 23.8 / 2
            + " cm\nmaxLeft: " + GlobalControl.Instance.leftCal * 43.3 / 2
            + " cm\nmaxRight: \n" + GlobalControl.Instance.rightCal * 43.3 / 2 + " cm"; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
