using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {

    [SerializeField]
    private GameObject controller;

    [SerializeField]
    private Image cob;

    private bool straightOutCal, crossedOutCal;

    private ControllerHandler ctrlr;

    private Vector2 leftFarthestCOB, rightFarthestCOB;

    private Vector3 leftFarthestHand, rightFarthestHand;

	// Use this for initialization
	void Start () {
        straightOutCal = false;
        crossedOutCal = false;

        ctrlr = (ControllerHandler)controller.GetComponent(typeof(ControllerHandler));
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 posn = Wii.GetCenterOfBalance(0);

		if (!straightOutCal)
        {
            if (ctrlr.isTriggerDown())
            {
                // grab c.o.b. and hand posn - assume right handed and switch if not later
                rightFarthestHand = controller.transform.position;
                rightFarthestCOB = posn;
                straightOutCal = true;

            }
        }
        else if (!crossedOutCal)
        {
            if (ctrlr.isTriggerDown())
            {
                if (posn.x < 0)
                {
                    leftFarthestHand = controller.transform.position;
                    leftFarthestCOB = posn;
                    Debug.Log("Right handed.");
                }
                else
                {
                    leftFarthestHand = rightFarthestHand;
                    leftFarthestCOB = rightFarthestCOB;

                    rightFarthestHand = controller.transform.position;
                    rightFarthestCOB = posn;
                    Debug.Log("left handed");
                }
                crossedOutCal = true;
            }

        }
        else
        {
            cob.transform.position = new Vector3(posn.x * 9f, posn.y * 4.5f , cob.transform.position.z);
        }
	}

    /// <summary>
    /// If the user is way outside of the position, it will be red.
    /// If the user is close, it will be yellow
    /// If the useris inside, it will be green
    /// </summary>
    private enum posnIndicator { RED, YELLOW, GREEN };
}
