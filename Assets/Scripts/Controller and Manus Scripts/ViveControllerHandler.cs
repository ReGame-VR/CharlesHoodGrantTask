using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveControllerHandler : MonoBehaviour {

    public Canvas canvas;
	// EW - The purpose of this script is unknown. it is not referenced and is outdated, so it will be commented out.

	// Controller and access to controller input
	// private SteamVR_TrackedObject ctrlr;
	//private SteamVR_Controller.Device Controller
	//{
	//	get { return SteamVR_Controller.Input((int)ctrlr.index); }
	//}

	// grab the tracked object component from the controllers - 
	// auto-attached in SteamVR prefab
	//void Awake()
 //   {
 //       ctrlr = GetComponent<SteamVR_TrackedObject>();
 //   }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Controller.GetHairTriggerDown())
    //    {
    //        canvas.GetComponent<Calibrate>().CaptureArmLengthShoulderHeight();
    //    }

    //    /* debug purposes
    //    if (Controller.GetHairTriggerDown())
    //    {
    //        Debug.Log(gameObject.name + " Trigger Press");
    //    }*/
    //}
}
