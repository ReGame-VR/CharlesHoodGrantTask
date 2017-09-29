﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class MenuController : MonoBehaviour {

    // public Button wiiButton;
    // public InputField participantID;

    public void ConnectWii()
    {
        Wii.StartSearch();
    }

    public void RecordID(string arg0)
    {
        Settings.Instance.participantID = arg0;
        Debug.Log(Settings.Instance.participantID);
    }

	// Use this for initialization
	void Start () {
        // disable VR settings for menu scene
        VRSettings.enabled = false;

        /* Button btn = wiiButton.GetComponent<Button>();
        btn.onClick.AddListener(ConnectWii);
        var se = new InputField.SubmitEvent();
        se.AddListener(RecordID); 
        */
	}
}
