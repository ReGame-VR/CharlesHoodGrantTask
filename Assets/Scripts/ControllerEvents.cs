using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEvents : MonoBehaviour {

    // define delegates and events

    // trigger the events

    // subscribing gameobjects

    public delegate void ControllerHandler(GameObject obj);

    public static event ControllerHandler onTriggerDown;

}
