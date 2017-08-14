using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOfBalance : MonoBehaviour {

    private bool isHeld;

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
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // draw position
        Gizmos.DrawCube( new Vector3(playerPosn.x * 10f +gizmoOffset.x, playerPosn.y * 10f + gizmoOffset.y, gizmoOffset.z), 
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

        if (!isHeld)
        {

        }
	}
}
