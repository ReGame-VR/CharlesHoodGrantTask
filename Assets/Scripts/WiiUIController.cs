using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WiimoteLib;

public class WiiUIController : MonoBehaviour {

    // displays on the UI where the 
    [SerializeField]
    private GameObject balancePointer;

    private Wiimote balanceBoard = new Wiimote();

	// Use this for initialization
	void Start () {
        try
        {
            balanceBoard.Connect();

        } catch(System.IO.IOException e)
        {
            Debug.Log(e);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (balanceBoard != null) {
            balancePointer.transform.position = new Vector3(balanceBoard.WiimoteState.BalanceBoardState.CenterOfGravity.X, 
                balanceBoard.WiimoteState.BalanceBoardState.CenterOfGravity.Y, balancePointer.transform.position.z);
        }
        else
        {
            Debug.Log("not connected.");
        }
	}
}
