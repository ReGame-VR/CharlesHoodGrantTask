using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

    private bool enteredID = false;

    public Text warning;

    public void ConnectWii()
    {
        Wii.StartSearch();
    }

    public void RecordID(string arg0)
    {
        Settings.Instance.participantID = arg0;
        enteredID = true;
        // Debug.Log(Settings.Instance.participantID);
    }

    public void SetRotation(bool rotating)
    {
        Settings.Instance.rotating = rotating;
    }

    /*public void SetRandomize(bool sequence)
    {
        isSequence = sequence;
    }*/

    public void SetHand(int input)
    {
        switch (input)
        {
            case 0:
                Settings.Instance.hand = Settings.Hand.RIGHT;
                break;
            case 1:
                Settings.Instance.hand = Settings.Hand.LEFT;
                break;
        }
    }

    public void NextScene()
    {
        if (!enteredID || Wii.GetRemoteCount() == 0)
        {
            string errorMessage = "One or more errors in calibration:\n";

            if (!enteredID)
            {
                errorMessage += "Participant ID not assigned.\n";
            }
            if (Wii.GetRemoteCount() == 0)
            {
                errorMessage += "Wii balance board not connected.";
            }

            warning.gameObject.SetActive(true);
            warning.text = errorMessage;
        }
        else
        {
            SceneManager.LoadScene("NEWCallibration");
        }
    }

	// Use this for initialization
	void Start () {
        // disable VR settings for menu scene
        UnityEngine.XR.XRSettings.enabled = false;
        warning.gameObject.SetActive(false);
	}
}
