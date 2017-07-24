using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CallibrationController : MonoBehaviour
{
    // x-distance from head to each controller
    private double leftreach, rightreach;

    // where the head is 
    [SerializeField]
    private Camera head;

    // the appropriate 
    [SerializeField]
    private Text correspondingText;

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
        if (Controller.GetHairTriggerDown())
        {
            setText(getDistanceTo());
        }
    }

    private double getDistanceTo()
    {
        float headx = head.transform.position.x;

        float controllerx = ctrlr.transform.position.x;
        
        return Mathf.Abs(headx - controllerx);
    }

    private void setText(double distance)
    {
        correspondingText.text = "Distance: " + distance;
    }
}
