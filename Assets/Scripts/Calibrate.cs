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

    private float forwardMax, backwardsMax, rightMax, leftMax;

    private Vector3 canvasOffset;

	// Use this for initialization
	void Start () {
        canvasOffset = COB.GetComponent<RectTransform>().position;
        forwardMax = 0.01f;
        backwardsMax = 0.01f;
        rightMax = 0.01f;
        leftMax = 0.01f;
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 posn = Wii.GetCenterOfBalance(0);
        Debug.Log(posn);

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

        COB.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + posn.x * 4.5f, canvasOffset.y + posn.y * 2f, canvasOffset.z);

        // horizontal bars
        forwardImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + forwardMax * 2f, canvasOffset.z);
        backwardsImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x, canvasOffset.y + backwardsMax * 2f, canvasOffset.z);

        // vertical bars
        leftImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + leftMax * 4.5f, canvasOffset.y, canvasOffset.z);
        rightImg.GetComponent<RectTransform>().position = new Vector3(canvasOffset.x + rightMax * 4.5f, canvasOffset.y, canvasOffset.z);

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
    }
}
