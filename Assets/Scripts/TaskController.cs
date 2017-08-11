using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskController : MonoBehaviour {
    /// randomly generate numbers - chooses which target will come up
    private System.Random rand = new System.Random();

    /// an array of positions where the targets are
    private Vector3[] posns = new Vector3[6];

    [SerializeField]
    private Text text;

    private bool isActive;

	// Use this for initialization
	void Start () {
		// TODO: CREATE 6 POSITIONS FOR THEM 
	}
	
	// Update is called once per frame
	void Update () {
        text.text = Wii.GetCenterOfBalance(0).ToString();
	}

    /// <summary>
    /// A helper function for determining which of the 6 targets will become active next
    /// </summary>
    /// <returns> 
    /// The index of the target to use
    /// </returns>
    private int getNextTarget()
    {
        return rand.Next(0, 6);
    }
}
