using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WiiBuddyTest : MonoBehaviour
{
    // public GameObject wiiObject;
    static int whichRemote;
    public float speed;

    void FixedUpdate()
    {
        if (Wii.IsActive(whichRemote)) //remote is on
        {
            if (Wii.GetExpType(whichRemote) == 3) //balance board is being used
            {
                Vector2 theCenter = Wii.GetCenterOfBalance(whichRemote);
                // Debug.Log("the total weight is: k" + Wii.GetTotalWeight(whichRemote));
                Debug.Log("COB:" + theCenter);
                // move self according the center of gravity
                //GetComponent<Rigidbody>().AddForce(new Vector3(theCenter.x * speed, 0, theCenter.y * speed));
            }
        }
    }

    void OnWiimoteFound(int thisRemote)
    {
        Debug.Log("found this one: " + thisRemote);
        if (!Wii.IsActive(whichRemote))
            whichRemote = thisRemote;
    }
}
