using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Calibrate : MonoBehaviour {

    public Image forwardImg;

    public Image backwardsImg;

    public Image rightImg;

    public Image leftImg;

    public Image COB;

    public GameObject hmd;

    public GameObject rightHand;

    public GameObject leftHand;

    public Text text;

    private float forwardMax, backwardsMax, rightMax, leftMax, forwardReach, backwardsReach;

    private Vector3 canvasOffset;

	// Use this for initialization
	void Start () {
        canvasOffset = COB.GetComponent<RectTransform>().position;
        forwardMax = 0f;
        backwardsMax = 0f;
        rightMax = 0f;
        leftMax = 0f;
        forwardReach = float.NaN;
        backwardsReach = float.NaN;
	}
	
	// Update is called once per frame
	void Update () {

        // - - - - - COB CALIBRATION - - - - -
        Vector2 posn = Wii.GetCenterOfBalance(0);

        if (posn.x > rightMax)
        {
            rightMax = posn.x;
        }

        if (posn.x < leftMax)
        {
            leftMax = posn.x;
        }

        if (posn.y > forwardMax)
        {
            forwardMax = posn.y;
        }

        if (posn.y < backwardsMax)
        {
            backwardsMax = posn.y;
        }
        
        // - - - - - REACH CALIBRATION - - - - -
        float hmdZ = hmd.transform.position.z;
        float hmdY = hmd.transform.position.y;
        float handZ, handY;

        // right or left handed?
        if (rightHand.activeInHierarchy)
        {
            handZ = rightHand.transform.position.z;
            handY = rightHand.transform.position.y;
        }
        else
        {
            handZ = leftHand.transform.position.z;
            handY = leftHand.transform.position.y;
        }

        // only count hand measurement if it is in a reasonable range to hold out
        if (handY > hmdY - 0.5f && handY < hmdY)
        {
            // Debug.Log("Entered loop");

            if (handZ > forwardReach || float.IsNaN(forwardReach) )
            {
                forwardReach = handZ;
                Debug.Log("Why");
            }
            else if (handZ < backwardsReach || float.IsNaN(backwardsReach))
            {
                backwardsReach = handZ;
            }
        }

        COB.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + posn.x * 4.5f, canvasOffset.y + posn.y * 2f, canvasOffset.z);

        // horizontal bars
        forwardImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + forwardMax * 2f, canvasOffset.z);
        backwardsImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + backwardsMax * 2f, canvasOffset.z);

        // vertical bars
        leftImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + leftMax * 4.5f, canvasOffset.y, canvasOffset.z);
        rightImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + rightMax * 4.5f, canvasOffset.y, canvasOffset.z);

        text.text = "Back reach: " + backwardsReach + "\n Front reach: " + forwardReach;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveData();
            SceneManager.LoadScene("Test");
        }
    }

    public void SaveData()
    {
        GlobalControl.Instance.forwardCal = forwardMax;
        GlobalControl.Instance.backwardsCal = backwardsMax;
        GlobalControl.Instance.leftCal = leftMax;
        GlobalControl.Instance.rightCal = rightMax;
        GlobalControl.Instance.backReach = backwardsReach;
        GlobalControl.Instance.frontReach = forwardReach;
    }
}
