using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Callibrate : MonoBehaviour {
    // the maximum that the user can lean in each direction
    private float maxLeft, maxRight, maxFront, maxBack, width, height;

    [SerializeField]
    private Text instructions;

    [SerializeField]
    private GameObject marker;

    // whether the 
    private bool isFrontCal, isBackCal, isLeftCal, isRightCal;

    // Controller and access to controller input
    private SteamVR_TrackedObject ctrlr;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)ctrlr.index); }
    }

    // grab the tracked object component from the controllers - 
    // auto-attached in SteamVR prefab
    void Awake()
    {
        ctrlr = GetComponent<SteamVR_TrackedObject>();

        isFrontCal = false;
        isBackCal = false;
        isLeftCal = false;
        isRightCal = false;
        width = 400f;
        height = 240f;
    }
	
	// Update is called once per frame
	void Update () {
        if (Wii.GetRemoteCount() == 0)
        {
            SetInstructions("Board not connected.");
            // Wii.StartSearch();
        }
        else
        {
            Vector2 pos = Wii.GetCenterOfBalance(0);

            if (!isFrontCal)
            {
                SetInstructions("Lean all the way forward and press the trigger");
                if (Controller.GetHairTriggerDown())
                {
                    maxFront = pos.y;
                    isFrontCal = true;
                }
            }
            else if (!isBackCal)
            {
                SetInstructions("Lean all the way back and press the trigger.");
                if (Controller.GetHairTriggerDown())
                {
                    maxBack = pos.y;
                    isBackCal = true;
                }
            }
            else if (!isLeftCal)
            {
                SetInstructions("Lean all the way to the left and press the trigger.");
                if (Controller.GetHairTriggerDown())
                {
                    maxLeft = pos.x;
                    isLeftCal = true;
                }
            }
            else if (!isRightCal)
            {
                SetInstructions("Lean all the way to the right and press the trigger.");
                if (Controller.GetHairTriggerDown())
                {
                    maxRight = pos.x;
                    isRightCal = true;
                }
            }
            else
            {
                SetInstructions("");
                ShowCenterOfBalance(pos);
            }
        }
    }

    // sets the text component
    private void SetInstructions(string str)
    {
        instructions.text = str;
    }

    private void ShowCenterOfBalance(Vector2 pos)
    {
        float pos_x, pos_y;

        if (pos.x > 0)
        {
            pos_x = (pos.x / maxLeft) * width;
        }
        else
        {
            pos_x = - (pos.x / maxRight) * width;
        }

        if (pos.y > 0)
        {
            pos_y = (pos.y / maxFront) * height;
        }
        else
        {
            pos_y = - (pos.y / maxBack) * height;
        }

        marker.GetComponent<RectTransform>().anchoredPosition = new Vector3(pos_x, pos_y, -10);
    }
}
