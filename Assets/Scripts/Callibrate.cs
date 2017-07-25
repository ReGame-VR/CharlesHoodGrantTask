using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Callibrate : MonoBehaviour {
    // easier to assign in inspector than get through code
    [SerializeField]
    private GameObject leftController, rightController, head;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private double getDistanceTo(WhichTransform which)
    {
        float headx = head.transform.position.x;

        switch (which)
        {
            case WhichTransform.left:
                return Mathf.Abs(headx - leftController.transform.position.x);
            case WhichTransform.right:
                return Mathf.Abs(headx - leftController.transform.position.x);
            // default case is find height of head
            default:
                return head.transform.position.y;
        }
    }

    private enum WhichTransform { left, right, head };
}
