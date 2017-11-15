﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles monitering and recording calibration values for the calibration scene. 
/// Currently records the following values:
///  - forward COB maximum
///  - backwards COB maximum
///  - right COB maximum
///  - left COB maximum
///  - arm length
///  - shoulder height
///  - left reach max
///  - right reach max
/// </summary>
public class Calibrate : MonoBehaviour {

    // UI images for calibration GUI
    public Image forwardImg;
    public Image backwardsImg;
    public Image rightImg;
    public Image leftImg;

    // UI Text for calibration GUI
    public Text forwardText;
    public Text backwardText;
    public Text rightText;
    public Text leftText;

    // UI image displaying user's current COB
    public Image COB;

    // the head mounted display - Camera (head) in [CameraRig] prefab
    public GameObject hmd;

    // Controller (left) and Controller(right) - give both, 1 will be active
    public GameObject rightHand;
    public GameObject leftHand;

    // public Text text;

    // Calibration values
    private float forwardMax, backwardsMax, rightMax, leftMax, armLen, shoulderHeight, 
        leftReach, rightReach;

    // The positional offset of the canvas at start
    private Vector3 canvasOffset;

	/// <summary>
    /// Initialize calibration values to zero.
    /// </summary>
	void Start () {
        canvasOffset = COB.GetComponent<RectTransform>().position;
        forwardMax = 0f;
        backwardsMax = 0f;
        rightMax = 0f;
        leftMax = 0f;
        leftReach = 0;
        rightReach = 0;
        armLen = float.NaN;
        shoulderHeight = float.NaN;
	}
	
	/// <summary>
    /// Check for new calibration maximums and update GUI. Also checks for key input from operator.
    /// Press left mouse button to record shoulder height and arm length, and press space to move
    /// onto next scene (only allowed if shoulder height and arm length have been recorded).
    /// </summary>
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

        // - - - - SIDE REACH CALIBRATION - - - -
        Vector3 posn3;

        // get controller position
        if (rightHand.activeInHierarchy)
        {
            posn3 = rightHand.transform.position;
        }
        else
        {
            posn3 = leftHand.transform.position;
        }

        // check to see if it is a new maximum
        if (posn3.x > rightReach)
        {
            rightReach = posn3.x;
        }
        else if (posn3.x < leftReach)
        {
            leftReach = posn3.x;
        }

        forwardText.text = forwardMax.ToString();
        backwardText.text = backwardsMax.ToString();
        rightText.text = rightMax.ToString();
        leftText.text = leftMax.ToString();

        // mouse button down to capture arm length and shoulder height
        if (Input.GetMouseButtonUp(0))
        {

            if (rightHand.activeInHierarchy)
            {
                armLen = rightHand.transform.position.z - hmd.transform.position.z;
                shoulderHeight = rightHand.transform.position.y - hmd.transform.position.y;
            }
            else
            {
                armLen = leftHand.transform.position.z - hmd.transform.position.z;
                shoulderHeight = leftHand.transform.position.y - hmd.transform.position.y;
            }
        }
        
        // Handle any changes to GUI
        COB.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + posn.x * 4.5f, canvasOffset.y + posn.y * 2f, canvasOffset.z);

        // horizontal bars
        forwardImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + forwardMax * 2f, canvasOffset.z);
        backwardsImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + backwardsMax * 2f, canvasOffset.z);

        // vertical bars
        leftImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + leftMax * 4.5f, canvasOffset.y, canvasOffset.z);
        rightImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + rightMax * 4.5f, canvasOffset.y, canvasOffset.z);

        // text
        forwardText.GetComponent<RectTransform>().position = forwardImg.GetComponent<RectTransform>().position;
        backwardText.GetComponent<RectTransform>().position = backwardsImg.GetComponent<RectTransform>().position;
        rightText.GetComponent<RectTransform>().position = rightImg.GetComponent<RectTransform>().position;
        leftText.GetComponent<RectTransform>().position = leftImg.GetComponent<RectTransform>().position;

        // text.text = "Arm Length: " + armLen.ToString();

        // Press space to load scene -- only go onto next scene if armLen has been calibrated
        if (Input.GetKeyDown(KeyCode.Space) && !float.IsNaN(armLen))
        {
            SaveData(); // Save data to GameControl
            SceneManager.LoadScene("Task");
        }
    }

    /// <summary>
    /// Save data to the singleton GameControl class. 
    /// </summary>
    public void SaveData()
    {
        GlobalControl.Instance.forwardCal = this.forwardMax;
        GlobalControl.Instance.backwardsCal = this.backwardsMax;
        GlobalControl.Instance.leftCal = this.leftMax;
        GlobalControl.Instance.rightCal = this.rightMax;
        GlobalControl.Instance.armLength = this.armLen;
        GlobalControl.Instance.shoulderHeight = this.shoulderHeight;
        GlobalControl.Instance.maxLeftReach = this.leftReach;
        GlobalControl.Instance.maxRightReach = this.rightReach;
    }
}
