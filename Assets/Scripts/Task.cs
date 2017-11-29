using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main controller for the task.
/// </summary>
public class Task : MonoBehaviour {
    // Event + Delegate for data recording
    public delegate void DataRecording(int trialNum, float time, int targetNum, float targetTime,
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence,
            Vector2 weightPosn, float COPTotalPath, float targetScore, float trialScore,
            float cumulativeScore);

    public static DataRecording OnRecordData;

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

    // The left controller (to be replaced with glove)
    public GameObject leftController;

    // The right controller (to be replaced with glove)
    public GameObject rightController;

    // time per trial
    private const float timePerTarget = 10f;

    // current time of current target
    private float curTime = 0f;
    
    // targets per trial
    private const int targetsPerTrial = 5;

    // current target in trial block
    private int curTarget = 1;

    // number of trials
    private const int numTrials = 10;

    // current trial number
    private int curTrial = 1;

    // score per target
    private float targetScore = 0;

    // score per trial
    private float trialScore = 0;

    // cumulative score 
    private float cumulativeScore;

    // the target order when a trial is a sequnce trial
    private int[] sequence = new int[5] { 3, 0, 2, 4, 1 };

    // for recording COB distance per target
    private Vector2 lastPosn;
    private float COBdistance = 0;

    // current target index
    private int targetIndex;

    // is the trial random or the set sequence?
    private bool isSequence;

    // data recording trials
    // what is this?
    private float time = 0f;

    // was the target touched?
    private bool touched = false;

    // Random
    System.Random rand = new System.Random();

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

        // get starting posn
        lastPosn = CoPtoCM(Wii.GetCenterOfBalance(0));
    }
	
	/// <summary>
    /// Run trials.
    /// </summary>
	void Update () {
        time += Time.deltaTime;

        // are the trials still running?
        if (curTrial <= numTrials) {
            // tick up clock
            curTime += Time.deltaTime;

            // get position
            Vector2 posn = CoPtoCM(Wii.GetCenterOfBalance(0));

            calculateDistances(posn);

            if (curTime < timePerTarget)
            {
                if (targets[targetIndex].indication == Target.posnIndicator.GREEN)
                {
                    // TODO: since target is green, is user touching target?
                }
            }
            else
            {
                ResetTarget(posn);
            }
        }
        else
        {
            // TODO: handle endgame
        }
    }

    /// <summary>
    /// Converts COP ratio to be in terms of cm to match PE task.
    /// </summary>
    /// <param name="posn"> The current COB posn, not in terms of cm </param>
    /// <returns> The posn, in terms of cm </returns>
    private Vector2 CoPtoCM(Vector2 posn)
    {
        return new Vector2(posn.x * 43.3f / 2f, posn.y * 23.6f / 2f);
    }

    /// <summary>
    /// Calculate distance moved since last frame and add it to COB distance.
    /// </summary>
    /// <param name="posn">The current COB position</param>
    private void calculateDistances(Vector2 posn)
    {
        COBdistance += Mathf.Sqrt(Mathf.Pow(posn.x - lastPosn.x, 2) 
            + Mathf.Pow(posn.y - lastPosn.y, 2));

        lastPosn = posn;
    }

    /// <summary>
    /// Whether target was touched or time is up, reset time values, distance, and advance to next
    /// trial.
    /// </summary>
    /// <param name="posn"> The current COB posn IN TERMS OF CM</param>
    private void ResetTarget(Vector2 posn)
    {
        if (OnRecordData != null)
        {
            // note: convert target index to a one-indexed value for data recording
            OnRecordData(curTrial, time, targetIndex + 1, curTime,
                (targets[targetIndex].indication == Target.posnIndicator.GREEN), touched, isSequence,
                posn, COBdistance, targetScore, trialScore, cumulativeScore);
        }

        curTime = 0;
        targetScore = 0;
        COBdistance = 0;
        touched = false;

        if (curTarget < targetsPerTrial) {
            curTarget++;
            if (isSequence)
            {
                targetIndex = sequence[curTarget];
            }
            else
            {
                targetIndex = rand.Next(6);
            }
        }

        else
        {
            ResetTrial();
        }
    }

    /// <summary>
    /// After the specified number of targets per trial is up, reset trial parameters and move onto
    /// the next trial.
    /// </summary>
    private void ResetTrial()
    {
        curTarget = 1;
        trialScore = 0;
        chooseTrialType();

        curTrial++;
    }

    /// <summary>
    /// Choose whether the set of 5 targets will be random or sequence.
    /// </summary>
    private void chooseTrialType()
    {
        int r = rand.Next(2);

        isSequence = (r == 1);
    }

    /// <summary>
    /// Checks the user's position against the target's assigned activation position, and makes
    /// any appropriate color changes.
    /// </summary>
    /// <param name="userPosn"> the user's position on the Wii board. </param>
    private void checkPosn(Vector2 userPosn)
    {
        // indicator is green, user can touch target
        if ((userPosn.x <= targets[targetIndex].CoPTarget.x + halfdim) 
            && (userPosn.x >= targets[targetIndex].CoPTarget.x - halfdim)
            && (userPosn.y <= targets[targetIndex].CoPTarget.y + halfdim)
            && (userPosn.y >= targets[targetIndex].CoPTarget.y - halfdim))
        {
            targets[targetIndex].isGreen();
        }
        // indicator is yellow
        else if ((userPosn.x <= targets[targetIndex].CoPTarget.x + devdim)
            && (userPosn.x >= targets[targetIndex].CoPTarget.x - devdim)
            && (userPosn.y <= targets[targetIndex].CoPTarget.y + devdim)
            && (userPosn.y >= targets[targetIndex].CoPTarget.y - devdim))
        {
            targets[targetIndex].isYellow();
        }
        // indicator is red
        else
        {
            targets[targetIndex].isRed();
        }
    }
}
