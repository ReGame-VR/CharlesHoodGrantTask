using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds functions for responding to and recording preferences on menu.
/// </summary>
public class MenuController : MonoBehaviour {

    // boolean that keeps track whether a participant ID has been entered at least once. This
    // script disallows moving to next scene if it hasn't
    private bool enteredID = false;

    // activates a text block that displays a warning if moving onto 
    public Text warning;

    // mostly for debugging and might want to remove - displays if wii is connected.
    public Text isWiiConnected;

    /// <summary>
    /// Attempts to find a Wii remote and connect it.
    /// TODO: troubleshoot issue where this method of connection does not work
    /// </summary>
    public void ConnectWii()
    {
        Wii.StartSearch();
    }

    /// <summary>
    /// Records an alphanumeric participant ID. Hit enter to record. May be entered multiple times
    /// but only last submission is used.
    /// </summary>
    /// <param name="arg0"></param>
    public void RecordID(string arg0)
    {
        GlobalControl.Instance.participantID = arg0;
        enteredID = true;
        // Debug.Log(Settings.Instance.participantID);
    }

    /// <summary>
    /// Records an integer indicating how many sequences of 5 targets are going to 
    /// appear in the task.
    /// </summary>
    /// <param name="arg0"></param>The entered integer
    public void RecordSequenceNum(string arg0)
    {
        int intAnswer = int.Parse(arg0);
        GlobalControl.Instance.numSequences = intAnswer;
    }

    /// <summary>
    /// Sets bool value that determines if targets are stationary or rotating.
    /// </summary>
    /// <param name="rotating"></param>
    public void SetRotation(bool rotating)
    {
        GlobalControl.Instance.isRotation = rotating;
    }

    /// <summary>
    /// Sets bool value that determines if participant is right handed
    /// </summary>
    /// <param name="rotating"></param>
    public void SetRightHanded(bool rightHanded)
    {
        GlobalControl.Instance.rightHanded = rightHanded;
    }

    /// <summary>
    /// Loads next scene if wii is connected and participant ID was entered.
    /// </summary>
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

	/// <summary>
    /// Disable VR for menu scene and hide warning text until needed.
    /// </summary>
	void Start () {
        // disable VR settings for menu scene
        UnityEngine.XR.XRSettings.enabled = false;
        warning.gameObject.SetActive(false);
        // Debug.Log(Wii.GetRemoteCount());
	}

    /// <summary>
    /// Update can be removed once Wii issue is resolved and whether or not Wii is connected does 
    /// not need to be displayed.
    /// </summary>
    void Update()
    {
        if (Wii.GetRemoteCount() == 0)
        {
            isWiiConnected.text = "Wii not connected.";
        }
        else
        {
            isWiiConnected.text = "Wii connected.";
        }
    }

    /// <summary>
    /// Re-enable VR when this script is disabled (since it is disabled on moving into next scene).
    /// </summary>
    void OnDisable()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }
}
