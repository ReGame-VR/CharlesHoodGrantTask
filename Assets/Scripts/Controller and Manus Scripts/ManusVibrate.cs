using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManusVR;

public class ManusVibrate : MonoBehaviour {

    
    public ManusVR.HandData handData;
	
	// Update is called once per frame
	void Update () {

    }

    public void VibrateSoftRight()
    {
        ManusVR.Manus.ManusSetVibration(handData.Session, ManusVR.device_type_t.GLOVE_RIGHT, 0.7f, 150);
    }

    public void VibrateSoftLeft()
    {
        ManusVR.Manus.ManusSetVibration(handData.Session, ManusVR.device_type_t.GLOVE_LEFT, 0.7f, 150);
    }
}
