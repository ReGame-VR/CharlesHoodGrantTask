using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManusVR;

public class ManusVibrate : MonoBehaviour {
        
    public HandData handData;

    public void VibrateSoftRight()
    {
        Manus.ManusSetVibration(handData.Session, device_type_t.GLOVE_RIGHT, 0.8f, 150);
    }

    public void VibrateSoftLeft()
    {
        Manus.ManusSetVibration(handData.Session, device_type_t.GLOVE_LEFT, 0.8f, 150);
    }

    public void VibrateVerySoftRight()
    {
        Manus.ManusSetVibration(handData.Session, device_type_t.GLOVE_RIGHT, 0.5f, 300);
    }

    public void VibrateVerySoftLeft()
    {
        Manus.ManusSetVibration(handData.Session, device_type_t.GLOVE_LEFT, 0.5f, 300);
    }
}
