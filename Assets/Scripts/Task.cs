using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour {

    // TODO:: change to short or tall if not doing custom ranges
    private float XMaxWeight = GlobalControl.Instance.rightCal;

    private float XMinWeight = GlobalControl.Instance.leftCal;

    private float YMaxWeight = GlobalControl.Instance.forwardCal;

    private float YMinWeight = GlobalControl.Instance.backwardsCal;

    private float backReach = GlobalControl.Instance.backReach;

    private float frontReach = GlobalControl.Instance.frontReach;

    private float dim, halfdim, devdim;

    private Target[] targets = new Target[6];

    public Text text;

    public GameObject cameraRig;

    public GameObject target;

    private ControllerHandler ctrlr;

    private float reachRange, weightShiftRange;

	// Use this for initialization
	void Start () {
        dim = 6f;

        halfdim = dim / 2;

        devdim = dim + 3;

        reachRange = frontReach - backReach;

        weightShiftRange = YMaxWeight - YMinWeight;

        Vector2 a, b, c, d, e, f;

        // create the 2d positions for the targets on the balance board
        a = new Vector2(XMinWeight / 2, YMaxWeight - halfdim);
        b = new Vector2(XMaxWeight - halfdim, YMaxWeight - halfdim);
        c = new Vector2(XMinWeight + halfdim, 0);
        d = new Vector2(2 + halfdim, 0);
        e = new Vector2(0 - halfdim, YMinWeight + halfdim);
        f = new Vector2((XMaxWeight - 2) - halfdim, YMinWeight + halfdim);

        // create the targets - 3d posns based on 2d posns 
        // A
        targets[0] = new Target(new Vector3(0, 0, 0), a);
        // B
        targets[1] = new Target(new Vector3(0, 0, 0), b);
        // C
        targets[2] = new Target(new Vector3(0, 0, 0), c);
        // D
        targets[3] = new Target(new Vector3(0, 0, 0), d);
        // E
        targets[4] = new Target(new Vector3(0, 0, 0), e);
        // F
        targets[5] = new Target(new Vector3(0, 0, 0), f);
	}
	
	// Update is called once per frame
	void Update () {
        Vector2 posn = CoPtoCM(Wii.GetCenterOfBalance(0));

        text.text = posn.ToString();
	}

    private Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }
}
