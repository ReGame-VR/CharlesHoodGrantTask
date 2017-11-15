using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main controller for the task.
/// </summary>
public class Task : MonoBehaviour {

    // TODO:: change to short or tall if not doing custom ranges
    private float XMaxWeight = GlobalControl.Instance.rightCal;

    private float XMinWeight = GlobalControl.Instance.leftCal;

    private float YMaxWeight = GlobalControl.Instance.forwardCal;

    private float YMinWeight = GlobalControl.Instance.backwardsCal;

    private float armLen = GlobalControl.Instance.armLength;

    private float shoulderHeight = GlobalControl.Instance.shoulderHeight;

    private float maxReachRight = GlobalControl.Instance.maxRightReach;

    private float maxReachLeft = GlobalControl.Instance.maxLeftReach;

    // parameters for weight shift accuracy
    private float dim, halfdim, devdim;

    // A list of 6 targets (1 for each position A-F)
    private Target[] targets = new Target[6];

    // The camerarig prefab
    public GameObject cameraRig;

    // The Camera (head) prefab of the camerarig
    public GameObject head;

    // The target prefab
    public GameObject target;

	/// <summary>
    /// Make calculations for 2D COB positions and 3D world positions based on calibration.
    /// </summary>
	void Start () {

        // calculate weightshitft as per PE and 2D VE
        dim = 6f;

        halfdim = dim / 2;

        devdim = dim + 3;

        float halfRange = armLen / 2;

        float height = head.transform.position.y;

        Vector2 a, b, c, d, e, f;

        // create the 2d positions for the targets on the balance board
        a = new Vector2(XMinWeight / 2, YMaxWeight - halfdim);
        b = new Vector2(XMaxWeight - halfdim, YMaxWeight - halfdim);
        c = new Vector2(XMinWeight + halfdim, 0);
        d = new Vector2(2 + halfdim, 0);
        e = new Vector2(0 - halfdim, YMinWeight + halfdim);
        f = new Vector2((XMaxWeight - 2) - halfdim, YMinWeight + halfdim);

        float max, mid, min, xposLeft, xposRight, depth;

        max = (shoulderHeight + shoulderHeight * 0.4f) * 1.4f;
        mid = shoulderHeight;
        min = shoulderHeight - shoulderHeight * -0.4f;

        xposLeft = maxReachLeft * 0.75f * 0.8f;

        xposRight = maxReachRight * 0.75f * 0.8f;

        depth = cameraRig.transform.position.z + armLen;

        // create the targets, giving 2 positions, 1 3d for position of target in scene, 1 2d 
        targets[0] = new Target(new Vector3(xposLeft*0.7f, min, depth), e);
        targets[1] = new Target(new Vector3(xposLeft, mid, depth), c);
        targets[2] = new Target(new Vector3(xposLeft, max, depth), a);
        targets[3] = new Target(new Vector3(xposRight, max, depth), b);
        targets[4] = new Target(new Vector3(xposRight, mid, depth), d);
        targets[5] = new Target(new Vector3(xposRight*0.7f, min, depth), f);

        // for testing -- real task, only need one
        GameObject t1, t2, t3, t4, t5, t6;

        t1 = Instantiate(target) as GameObject;
        t1.transform.position = targets[0].worldPosn;

        t2 = Instantiate(target) as GameObject;
        t2.transform.position = targets[1].worldPosn;

        t3 = Instantiate(target) as GameObject;
        t3.transform.position = targets[2].worldPosn;

        t4 = Instantiate(target) as GameObject;
        t4.transform.position = targets[3].worldPosn;

        t5 = Instantiate(target) as GameObject;
        t5.transform.position = targets[4].worldPosn;

        t6 = Instantiate(target) as GameObject;
        t6.transform.position = targets[5].worldPosn;
    }
	
	/// <summary>
    /// 
    /// </summary>
	void Update () {
        Vector2 posn = CoPtoCM(Wii.GetCenterOfBalance(0));
	}

    /// <summary>
    /// Converts COP ratio to be in terms of cm to match PE task.
    /// </summary>
    /// <param name="posn"></param> The current COB posn, not in terms of cm
    /// <returns></returns> The posn, in terms of cm
    private Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }
}
