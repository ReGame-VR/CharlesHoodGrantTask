using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Callibrate : MonoBehaviour {
    // the dimensions of the board displaying balance
    private float width, height;

    [SerializeField]
    private Text instructions;

    [SerializeField]
    private GameObject marker;

    private Vector3 initPosn;

    private bool isDown, isStraightOut, isCrossedOut;

    private Vector3 downPosn, outStraightPosn, outCrossedPosn;

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

        initPosn = marker.transform.position;
        SetInstructions("Put your hand at your side.");

        if (Wii.GetRemoteCount() == 0)
        {
            SetInstructions("Board not connected.");
            // Wii.StartSearch();
        }

        ctrlr = GetComponent<SteamVR_TrackedObject>();

        width = 400f;
        height = 240f;

        isDown = false;
        isStraightOut = false;
        isCrossedOut = false;
    }
	
	// Update is called once per frame
	void Update () {
       
        Vector2 pos = Wii.GetCenterOfBalance(0);

        SetInstructions("");
        ShowCenterOfBalance(pos);

        if (!isDown)
        {
            if (Controller.GetHairTriggerDown())
            {
                downPosn = ctrlr.transform.position;
                SetInstructions("Reach straight out.");
            }
        }
        else if (!isStraightOut)
        {
            if (Controller.GetHairTriggerDown())
            {
                outStraightPosn = ctrlr.transform.position;
                SetInstructions("Reach across your body.");
            }
        }
        else if (!isCrossedOut)
        {
            if (Controller.GetHairTriggerDown())
            {
                downPosn = ctrlr.transform.position;
                SetInstructions("");
            }
        }
        else
        {
            // do something here
                                                                                                                                 
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

        pos_x = (pos.x) * width;
        pos_y = (pos.y) * height;

        marker.GetComponent<RectTransform>().anchoredPosition = new Vector3(initPosn.x + pos_x, initPosn.y + pos_y, initPosn.z);
    }
}
