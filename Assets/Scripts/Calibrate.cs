using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Handles monitoring and recording calibration values for the calibration scene. 
/// Currently records the following values:
///  - forward center of balance maximum
///  - backward center of balance maximum
///  - right center of balance maximum
///  - left center of balance maximum
///  - arm length
///  - shoulder height
///  - left reach maximum
///  - right reach maximum
/// </summary>
public class Calibrate : MonoBehaviour {

    // UI images for calibration GUI
    public Image forwardImg;
    public Image backwardsImg;
    public Image rightImg;
    public Image leftImg;
    public Image COB;

    // UI Text for calibration GUI
    public Text forwardText;
    public Text backwardText;
    public Text rightText;
    public Text leftText;

    // the head mounted display - Camera (head) in [CameraRig] prefab
    public GameObject hmd;

    // Controller (left) and Controller(right) - give both, but only 1 will be active
    // TODO: Replace with Manus Gloves
    public GameObject rightHand;
    public GameObject leftHand;

    // Calibration values
    private float forwardMax, backwardsMax, rightMax, leftMax, armLen, shoulderHeight, 
        leftReach, rightReach;

    // The positional offset of the canvas at start
    private Vector3 canvasOffset;

	/// <summary>
    /// Initialize calibration values to zero, assigning NaN to arm length and shoulder height until
    /// they are recorded with a mouse press. 
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
        Vector2 posn = Task.CoPtoCM(Wii.GetCenterOfBalance(0));

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

        if (Input.GetMouseButtonUp(0))
        {
            this.CaptureArmLengthShoulderHeight();

        }
        
        // Handle any changes to GUI
        COB.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + posn.x * 0.2f, canvasOffset.y + posn.y * 0.2f, canvasOffset.z);

        // horizontal bars
        forwardImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + forwardMax * 0.2f, canvasOffset.z);
        backwardsImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + backwardsMax * 0.2f, canvasOffset.z);

        // vertical bars
        leftImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + leftMax * 0.2f, canvasOffset.y, canvasOffset.z);
        rightImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + rightMax * 0.2f, canvasOffset.y, canvasOffset.z);

        // text
        forwardText.GetComponent<RectTransform>().position = forwardImg.GetComponent<RectTransform>().position;
        backwardText.GetComponent<RectTransform>().position = backwardsImg.GetComponent<RectTransform>().position;
        rightText.GetComponent<RectTransform>().position = rightImg.GetComponent<RectTransform>().position;
        leftText.GetComponent<RectTransform>().position = leftImg.GetComponent<RectTransform>().position;

        // Press space to load scene -- only go onto next scene if armLen has been calibrated
        if (Input.GetKeyDown(KeyCode.Space) && !float.IsNaN(armLen))
        {
            SaveData(); // Save data to GameControl

            SceneManager.LoadScene("Task");
        }
    }

    /// <summary>
    /// Save calibration data to GlobalControl. 
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

    // Capture the arm length and shoulder height for calibration. Called using hair 
    // trigger on Vive or mouse up.
    public void CaptureArmLengthShoulderHeight()
    {
        if (rightHand.activeInHierarchy)
        {
            armLen = Mathf.Abs(rightHand.transform.position.z - hmd.transform.position.z);
            shoulderHeight = rightHand.transform.position.y;
        }
        else
        {
            armLen = Mathf.Abs(leftHand.transform.position.z - hmd.transform.position.z);
            shoulderHeight = leftHand.transform.position.y;
        }
        Debug.Log("Arm Length: " + armLen.ToString());

    }
}
