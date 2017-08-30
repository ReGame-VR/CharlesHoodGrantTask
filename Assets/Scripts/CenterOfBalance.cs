using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfBalance : MonoBehaviour {

    /// <summary>
    /// isHeld - whether the user is within the position
    /// straightOut - whether the user reaching straight has been calibrated
    /// crossedOut - whether the user reaching cross out has been calibrated
    /// back - whether the user reaching back has been calibrated
    /// </summary>
    private bool isHeld, straightOut, crossedOut, back;

    private Vector2 toHold, playerPosn;

    [SerializeField]
    [Range (0f, 1f)]
    private float errorMargin;

    // DELETE WHEN GIZMOS NOT NEEDED
    private Vector3 gizmoOffset;

    [SerializeField]
    private Vector2 sizeOfCanvas;

    // Use this for initialization
    void Start () {
        gizmoOffset = new Vector3(0f, 2.5f, 5f);
        toHold = new Vector2(0, 0);
        isHeld = false;
        straightOut = false;

	}

    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // draw position
        Gizmos.DrawCube( new Vector3(playerPosn.x * 9f +gizmoOffset.x, playerPosn.y * 4.5f + gizmoOffset.y, gizmoOffset.z), 
            new Vector3(0.1f, 0.1f, 0.1f) );

        // draw square around point where error margin is
        // top horizontal line
        Gizmos.DrawLine(new Vector3(toHold.x - errorMargin / 2  + gizmoOffset.x, toHold.y + errorMargin / 2 + gizmoOffset.y, gizmoOffset.z),
            new Vector3(toHold.x + errorMargin / 2 + gizmoOffset.x, toHold.y + errorMargin / 2 + gizmoOffset.y, gizmoOffset.z));

        // bottom horizontal line
        Gizmos.DrawLine(new Vector3(toHold.x - errorMargin / 2 + gizmoOffset.x, toHold.y - errorMargin / 2 + gizmoOffset.y, gizmoOffset.z),
            new Vector3(toHold.x + errorMargin / 2 + gizmoOffset.x, toHold.y  - errorMargin / 2 + gizmoOffset.y, gizmoOffset.z));

        // left vertical line
        Gizmos.DrawLine(new Vector3(toHold.x - errorMargin / 2 + gizmoOffset.x, toHold.y + errorMargin / 2 + gizmoOffset.y, gizmoOffset.z),
            new Vector3(toHold.x - errorMargin / 2 + gizmoOffset.x, toHold.y - errorMargin / 2 + gizmoOffset.y, gizmoOffset.z));

        // right vertical line
        Gizmos.DrawLine(new Vector3(toHold.x + errorMargin / 2 + gizmoOffset.x, toHold.y + errorMargin / 2 + gizmoOffset.y, gizmoOffset.z),
            new Vector3(toHold.x + errorMargin / 2 + gizmoOffset.x, toHold.y - errorMargin / 2 + gizmoOffset.y, gizmoOffset.z));
    }
	
	// Update is called once per frame
	void Update () {
        playerPosn = Wii.GetCenterOfBalance(0);
	}
}
