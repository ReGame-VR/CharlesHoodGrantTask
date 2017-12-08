﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class ViveControllerHandler : MonoBehaviour {

    // the object that a controller is colliding with
    // private GameObject touchedTarget;
    
    // Controller and access to controller input
    private SteamVR_TrackedObject ctrlr;

    private bool colliding;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int) ctrlr.index); }
    }

    // grab the tracked object component from the controllers - auto-attached in SteamVR prefab
    void Awake()
    {
        ctrlr = GetComponent<SteamVR_TrackedObject>();
        colliding = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!colliding) {
            colliding = true;
            GameObject touched = other.gameObject;

            if (touched.tag == "Target")
            {
                doCollisionStuff(touched.GetComponent<AudioSource>());
                toggleOutline(other.gameObject);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        colliding = false;
        toggleOutline(other.gameObject);

    }

    private void doCollisionStuff(AudioSource sound)
    {
        sound.Play();
    }

    private void toggleOutline(GameObject target)
    {
        target.GetComponent<Outline>().enabled = !target.GetComponent<Outline>().enabled;
    }
}