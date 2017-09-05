using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestTextController : MonoBehaviour {

    public Text text;

	// Use this for initialization
	void Start () {
        text.text = "max front:\nmax back: \nmaxLeft: \nmaxRight: \n"; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
