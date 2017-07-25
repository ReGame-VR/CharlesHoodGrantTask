using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallibrationController : MonoBehaviour
{
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
    }

    // Update is called once per frame
    void Update()
    {

    }
}
