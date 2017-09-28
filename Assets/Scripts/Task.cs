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

    // public Text text;

    public GameObject head;

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

        float ratio = reachRange / weightShiftRange;

        float halfRange = reachRange / 2;

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
        targets[0] = new Target(new Vector3(head.transform.position.x - 0.3f, 
            head.transform.position.y, a.y * ratio + halfRange), a);
        // B
        targets[1] = new Target(new Vector3(head.transform.position.x + 0.3f, 
            head.transform.position.y, b.y * ratio + halfRange), b);
        // C
        targets[2] = new Target(new Vector3(head.transform.position.x + 0.5f, 
            head.transform.position.y - 0.17f, c.y * ratio + halfRange), c);
        // D
        targets[3] = new Target(new Vector3(head.transform.position.x - 0.5f, 
            head.transform.position.y - 0.17f, d.y * ratio + halfRange), d);
        // E
        targets[4] = new Target(new Vector3(head.transform.position.x + 0.3f, 
            head.transform.position.y - 0.34f, e.y * ratio + halfRange), e);
        // F
        targets[5] = new Target(new Vector3(head.transform.position.x - 0.3f,
            head.transform.position.y - 0.34f, f.y * ratio + halfRange), f);

        // for testing
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
	
	// Update is called once per frame
	void Update () {
        Vector2 posn = CoPtoCM(Wii.GetCenterOfBalance(0));
	}

    private Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }
}
