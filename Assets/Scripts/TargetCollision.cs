using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCollision : MonoBehaviour {
    //private AudioSource audio;

    [SerializeField]
    private GameObject controller1;

    [SerializeField]
    private GameObject controller2;

    private Collider col1, col2;

    private bool isCollision;

    // Get colliders
    void Awake () {
        // audio = GetComponent<AudioSource>();
        col1 = controller1.GetComponent<Collider>();
        col2 = controller2.GetComponent<Collider>();
        isCollision = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (!isCollision)
        {
            // Debug.Log("Collision");
            if (other == col1 || other == col2)
            {
                OnTouch();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other == col1 || other == col2)
        {
            isCollision = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == col1 || other == col2)
        {
            isCollision = false;
        }
    }

    private void OnTouch()
    {
        isCollision = true;

        // Debug.Log("audio played");
        GetComponent<AudioSource>().Play();         
    }
}
