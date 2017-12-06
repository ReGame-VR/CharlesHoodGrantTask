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
        GlobalControl.Instance.participantID = arg0;
        enteredID = true;
        // Debug.Log(Settings.Instance.participantID);
    }

    public void SetRotation(bool rotating)
    {
        GlobalControl.Instance.isRotation = rotating;
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
            SceneManager.LoadScene("Calibrate");
        }
    }

	// Use this for initialization
	void Start () {
        // disable VR settings for menu scene
        UnityEngine.XR.XRSettings.enabled = false;
        warning.gameObject.SetActive(false);
	}
}
