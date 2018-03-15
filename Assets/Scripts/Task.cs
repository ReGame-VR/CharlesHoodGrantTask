using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The main controller for the task.
/// </summary>
public class Task : MonoBehaviour
{
    // The delegate that invokes recording of trial information
    public delegate void DataRecording(string participantId, bool rightHanded, 
            bool isRotation, int trialNum, float time, int targetNum, float targetTime,
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence,
            Vector2 weightPosn, float COPTotalPath, float trialScore,
            float cumulativeScore, float curGreenTime, float curYellowTime, float curRedTime);

    public static DataRecording OnRecordData;

    // The delegate that invokes recording of continuous values like CoP and CoM
    public delegate void ContinuousDataRecording(string participantId, float time, Vector2 CoPposition, Target.posnIndicator curColor,
            Vector3 CoMposition, int targetNum, int trialNum);

    public static ContinuousDataRecording OnRecordContinuousData;

    // TODO:: change to short or tall if not doing custom ranges
    private float XMaxWeight = GlobalControl.Instance.rightCal;

    private float XMinWeight = GlobalControl.Instance.leftCal;

    private float YMaxWeight = GlobalControl.Instance.forwardCal;

    private float YMinWeight = GlobalControl.Instance.backwardsCal;

    private float armLen = GlobalControl.Instance.armLength;

    private float shoulderHeight = GlobalControl.Instance.shoulderHeight;

    private float maxReachRight = GlobalControl.Instance.maxRightReach;

    private float maxReachLeft = GlobalControl.Instance.maxLeftReach;

    private bool isRotation = GlobalControl.Instance.isRotation;

    private bool rightHanded = GlobalControl.Instance.rightHanded;

    private string participantId = GlobalControl.Instance.participantID;

    // offset between middle and top/bottom in the physical environment
    private float midTargetOffset = 0.14f;

    // the radius of rotation - from center screw to center of target
    private float radius = 0.1025f;

    // parameters for weight shift accuracy
    private float dim, halfdim, devdim;

    // A list of 6 targets (1 for each position A-F) - positions generated in Start()
    private Target[] targets = new Target[6];

    // The camerarig prefab
    public GameObject cameraRig;

    // The Camera (head) prefab of the camerarig
    public GameObject head;

    // The target object to be moved in the scene
    public GameObject target;

    // the rotation object to be moved in the scene
    public GameObject rotationObj;

    // The left controller 
    //TODO: replace with glove
    public GameObject leftController;

    // The right controller
    //TODO: replace with glove
    public GameObject rightController;

    // the 3 materials for the target
    public Material redMat;
    public Material yellowMat;
    public Material greenMat;

    // Allows a way to send vibration messages
    public ManusVibrate vibrate;

    // The array of raycasting objects for each finger. Separate arrays for each hand.
    public FingerRaycaster[] fingerRayArrayLeft;
    public FingerRaycaster[] fingerRayArrayRight;

    // time per trial
    private const float timePerTarget = 10f;

    // current time of current target
    private float curTime = 0f;

    // current Time that the player spent in green, yellow, and red positions
    // for the current target
    private float curGreenTime = 0f;
    private float curYellowTime = 0f;
    private float curRedTime = 0f;

    // number of trials
    private int numTrials;

    // current trial number
    private int curTrial = 1;

    // score per trial
    private float trialScore = 0;

    // The size of each target group. During a target group, the targets will be spawned
    // either randomly or in a set sequence. 
    private int targetGroupSize = 5;

    // The current index of a target within a target grouping. When this reaches
    // the size of the target grouping, the grouping will change.
    private int curGroupNumber = 1;

    // cumulative score 
    private float cumulativeScore;

    // the target order when a trial is a sequnce trial - same as 2D PE, but 0 indexed
    private int[] sequence = new int[5] { 3, 0, 2, 4, 1 };

    // for recording COB distance per target
    private Vector2 lastPosn;
    private float COBdistance = 0;

    // current target index
    private int targetIndex = 0;

    // is the current target group a random sequence?
    private bool isRandomSequence = true;

    // data recording trials
    // global time
    private float time = 0f;

    // was the target touched?
    private bool touched = false;

    // for random number generation
    private System.Random rand = new System.Random();

    // Tells if the game is over
    private bool gameOver = false;

    // for assigning the correct material
    private Renderer rend;

    // The canvas displaying score during the task
    private TaskCanvas taskCanvas;

    void Awake()
    {
        taskCanvas = GetComponent<TaskCanvas>();
    }

    /// <summary>
    /// Make calculations for 2D COB positions and 3D world positions based on calibration.
    /// </summary>
    void Start()
    {
        // The number of trials is the size of each group (sequence) times
        // the number of sequences. The number of sequences was decided
        // in the UI in the Menu scene. 
        numTrials = targetGroupSize * GlobalControl.Instance.numSequences;

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

        // calculations for 3D posns
        float max, mid, min, xposLeft, xposRight, depth;

        max = shoulderHeight * 1.25f;
        mid = shoulderHeight;
        min = shoulderHeight * 0.75f;

        xposLeft = maxReachLeft + midTargetOffset;

        xposRight = maxReachRight - midTargetOffset;

        depth = cameraRig.transform.position.z + armLen * 1.2f; // armLen * 0.8f ?

        // create the targets
        targets[0] = new Target(new Vector3(xposLeft, min, depth), e);
        targets[1] = new Target(new Vector3(xposLeft - midTargetOffset, mid, depth), c);
        targets[2] = new Target(new Vector3(xposLeft, max, depth), a);
        targets[3] = new Target(new Vector3(xposRight, max, depth), b);
        targets[4] = new Target(new Vector3(xposRight + midTargetOffset, mid, depth), d);
        targets[5] = new Target(new Vector3(xposRight, min, depth), f);

        //Set up either the stationary target or the rotating target
        rend = SetUpTargetsAndRenderer();

        chooseTrialType();
        chooseNextTarget();
        placeTarget();

        // get starting posn
        lastPosn = CoPtoCM(Wii.GetCenterOfBalance(0));
    }

    /// <summary>
    /// Run trials.
    /// </summary>
    void FixedUpdate()
    {
        if (gameOver)
        {
            // Game is over, make targets inactive and check
            // to see if space was pressed to restart scene.
            rotationObj.SetActive(false);
            target.SetActive(false);
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                // The game is over, don't run any of the following code.
                return;
            }
        }

        time += Time.deltaTime;

        // tick up clock
        curTime += Time.deltaTime;

        // get position
        Vector2 posn = CoPtoCM(Wii.GetCenterOfBalance(0));

        // for data recording total distance of weight shift per target
        calculateDistances(posn);

        UpdateContinuousData(posn);

        // check to see if color of target should change
        checkPosn(posn);

        if (curTime < timePerTarget)
        {
            if (targets[targetIndex].indication == Target.posnIndicator.GREEN)
            {
                // Target is unlocked, check to see if player hit it
                CheckCollisions(posn);
            }
            else
            {
                // Target is still locked, check to see if player touched
                // it to give light haptic feedback
                CheckLockedCollisions();
            }
        }
        else
        {
            // failed to hit target in time
            GetComponent<SoundEffectPlayer>().PlayFailSound();
            TriggerFailParticles();
            ResetTarget(posn);
        }
    }

    /// <summary>
    /// Checks to see if the user's finger touched an unlocked target
    /// </summary>
    /// <param name="posn"></param> The parameter necessary for the ResetTarget method
    private void CheckCollisions(Vector2 posn)
    {
        FingerRaycaster[] fingerRayArray = GetCorrectFingerArray();

        foreach (FingerRaycaster fingerRaycaster in fingerRayArray)
        {
            Vector3 fingerPosition = fingerRaycaster.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(fingerPosition, Vector3.forward, out hit, 0.01f) && hit.collider.tag == "Target")
            {
                // The player hit the target! Calculate score, play success sound,
                // spawn success particles, give vibration feedback, reset target.
                float distanceFromCenter = findPointDistanceFromCenter(hit.point);
                trialScore = Target.ScoreTouch2(distanceFromCenter, curTime);
                touched = true;
                GetComponent<SoundEffectPlayer>().PlaySuccessSound();
                TriggerSuccessParticles();
                VibrateActiveController();
                ResetTarget(posn);
            }
        }
    }

    /// <summary>
    /// Checks if the player touched a LOCKED target
    /// </summary>
    private void CheckLockedCollisions()
    {
        FingerRaycaster[] fingerRayArray = GetCorrectFingerArray();

        foreach (FingerRaycaster fingerRaycaster in fingerRayArray)
        {
            Vector3 fingerPosition = fingerRaycaster.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(fingerPosition, Vector3.forward, out hit, 0.01f) && hit.collider.tag == "Target")
            {
                // The player touched a locked target so give light feedback
                VibrateActiveControllerVerySoftly();
            }
        }
    }

    /// <summary>
    /// Finds the distance from the center
    /// of a target with a raycast hit point.
    /// </summary>
    /// <param name="point"></param> The 3D raycast hit point
    /// <returns></returns>float that is the distance
    private float findPointDistanceFromCenter(Vector3 point)
    {
        Vector2 hit2d = new Vector2(point.x, point.y);

        if (GlobalControl.Instance.isRotation)
        {
            //gets the transform of the child of the rotating arm
            float x = rotationObj.transform.GetChild(0).position.x;
            float y = rotationObj.transform.GetChild(0).position.y;
            Vector2 centerOfMovingTarget = new Vector2(x, y);
            return Vector2.Distance(hit2d, centerOfMovingTarget);
        }
        else
        {
            float x = target.transform.position.x;
            float y = target.transform.position.y;
            Vector2 centerOfStationaryTarget = new Vector2(x, y);
            return Vector2.Distance(hit2d, centerOfStationaryTarget);
        }
    }

    /// <summary>
    /// Vibrates the controller on the correct hand.
    /// </summary>
    private void VibrateActiveController()
    {
        if (rightHanded)
        {
            vibrate.VibrateSoftRight();
        }
        else
        {
            vibrate.VibrateSoftLeft();
        }
    }

    /// <summary>
    /// Vibrates the controller on the correct hand (very softly).
    /// </summary>
    private void VibrateActiveControllerVerySoftly()
    {
        if (rightHanded)
        {
            vibrate.VibrateVerySoftRight();
        }
        else
        {
            vibrate.VibrateVerySoftLeft();
        }
    }

    /// <summary>
    /// Converts COP ratio to be in terms of cm to match PE task.
    /// </summary>
    /// <param name="posn"> The current COB posn, not in terms of cm </param>
    /// <returns> The posn, in terms of cm </returns>
    public static Vector2 CoPtoCM(Vector2 posn)
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
        // update trial score on canvas and update actual total score
        taskCanvas.UpdateTrialScoreText(trialScore);
        cumulativeScore = cumulativeScore + trialScore;

        // Record the data for this trial
        if (OnRecordData != null)
        {
            // note: convert target index to a one-indexed value for data recording
            OnRecordData(participantId, rightHanded, isRotation, curTrial, time, targetIndex + 1, curTime,
                (targets[targetIndex].indication == Target.posnIndicator.GREEN), touched, isRandomSequence,
                posn, COBdistance, trialScore, cumulativeScore, curGreenTime, curYellowTime, curRedTime);
        }

        curTime = 0;
        curGreenTime = 0f;
        curYellowTime = 0f;
        curRedTime = 0f;
        trialScore = 0;
        COBdistance = 0;
        touched = false;


        if ((curGroupNumber < targetGroupSize) && (curTrial < numTrials))
        {
            // Reset trial only. Same group, so same sequence.
            ResetTrial();
        }
        else if (curTrial < numTrials)
        {
            ResetTrialAndGroup();
        }
        // task is over
        else
        {
            TriggerGameOverParticles();
            taskCanvas.UpdateTotalScoreText(cumulativeScore);
            taskCanvas.EnableGameOverText();
            gameOver = true;
        }
    }

    /// <summary>
    /// After the trial is up, reset trial parameters and move onto
    /// the next trial.
    /// </summary>
    private void ResetTrial()
    {
        curGroupNumber++;
        curTrial++;
        trialScore = 0;

        chooseNextTarget();
        placeTarget();
    }

    /// <summary>
    /// After the trial is up, reset it. Also reset the group
    /// and choose a new kind of sequence for the next target
    /// group.
    /// </summary>
    private void ResetTrialAndGroup()
    {
        curGroupNumber = 0;
        curTrial++;
        trialScore = 0;

        chooseTrialType();
        chooseNextTarget();
        placeTarget();
    }

    /// <summary>
    /// Move either just the target or the gameobject with the rotation script and target to the
    /// stored location based on the current target index.
    /// </summary>
    private void placeTarget()
    {
        if (isRotation)
        {
            rotationObj.transform.position = targets[targetIndex].worldPosn;
        }
        else
        {
            Vector3 offset = -new Vector3(radius, 0, 0);
            if (targetIndex < 3)
            {
                target.transform.position = targets[targetIndex].worldPosn - offset;
            }
            else
            {
                target.transform.position = targets[targetIndex].worldPosn + offset;
            }
        }
    }

    /// <summary>
    /// Choose the next target either randomly or within the sequence
    /// </summary>
    private void chooseNextTarget()
    {
        if (isRandomSequence)
        {
            int prevTargetIndex = targetIndex;
            // Keep looking for a random target index that is different
            // from the previous targetIndex (We don't want the same
            // target to appear twice in a row).
            while (prevTargetIndex == targetIndex)
            {
                targetIndex = rand.Next(6);
            }
        }
        else
        {
            targetIndex = (targetIndex + 1) % targets.Length;
        }
    }

    /// <summary>
    /// Choose whether the set of 5 targets will be random or sequence (50% chance of each).
    /// </summary>
    private void chooseTrialType()
    {
        int r = rand.Next(2);

        isRandomSequence = (r == 1);
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
            setColor(Target.posnIndicator.GREEN);
        }
        // indicator is yellow
        else if ((userPosn.x <= targets[targetIndex].CoPTarget.x + devdim)
            && (userPosn.x >= targets[targetIndex].CoPTarget.x - devdim)
            && (userPosn.y <= targets[targetIndex].CoPTarget.y + devdim)
            && (userPosn.y >= targets[targetIndex].CoPTarget.y - devdim))
        {
            setColor(Target.posnIndicator.YELLOW);
        }
        // indicator is red
        else
        {
            setColor(Target.posnIndicator.RED);
        }
    }

    /// <summary>
    /// Sets the appropriate target's indication to the given color, and assigns the appropriate
    /// material to the target GameObject.
    /// </summary>
    /// <param name="indication">The color that the target should be.</param>
    private void setColor(Target.posnIndicator indication)
    {
        targets[targetIndex].indication = indication;

        switch (indication)
        {
            case Target.posnIndicator.GREEN:
                rend.material = greenMat;
                break;
            case Target.posnIndicator.YELLOW:
                rend.material = yellowMat;
                break;
            default:
                rend.material = redMat;
                break;
        }
    }

    /// <summary>
    /// Gets the renderer necessary to change color. Either the rotation renderer or
    /// the stationary renderer. Also disables the unnecessary target (rotating or not)
    /// </summary>
    /// <returns></returns> The correct renderer whose color to change.
    private Renderer SetUpTargetsAndRenderer()
    {
        if (isRotation)
        {
            target.SetActive(false);
            return rotationObj.GetComponentInChildren<Renderer>();
        }
        else
        {
            rotationObj.SetActive(false);
            return target.GetComponent<Renderer>();
        }
    }

    /// <summary>
    /// Gets the currently active fingerRayArray for collision checking
    /// </summary>
    /// <returns></returns> The left or right set of finger rays
    private FingerRaycaster[] GetCorrectFingerArray()
    {
        if (rightHanded)
        {
            return fingerRayArrayRight;
        }
        else
        {
            return fingerRayArrayLeft;
        }
    }
    
    /// <summary>
    /// Triggers the spawning of *success* particles on the attached particle spawner
    /// </summary>
    private void TriggerSuccessParticles()
    {
        ParticleSpawner spawner = GetComponent<ParticleSpawner>();

        if (GlobalControl.Instance.isRotation)
        {
            spawner.SpawnSuccessParticles(rotationObj.transform.GetChild(0).position);
        }
        else
        {
            spawner.SpawnSuccessParticles(target.transform.position);
        }
    }

    /// <summary>
    /// Triggers the spawning of *fail* particles on the attached particle spawner
    /// </summary>
    private void TriggerFailParticles()
    {
        ParticleSpawner spawner = GetComponent<ParticleSpawner>();

        if (GlobalControl.Instance.isRotation)
        {
            spawner.SpawnFailParticles(rotationObj.transform.GetChild(0).position);
        }
        else
        {
            spawner.SpawnFailParticles(target.transform.position);
        }
    }

    /// <summary>
    /// Triggers the spawning of game over particles on the attached particle spawner
    /// </summary>
    private void TriggerGameOverParticles()
    {
        ParticleSpawner spawner = GetComponent<ParticleSpawner>();

        if (GlobalControl.Instance.isRotation)
        {
            spawner.SpawnGameOverParticles(rotationObj.transform.GetChild(0).position);
        }
        else
        {
            spawner.SpawnGameOverParticles(target.transform.position);
        }
    }

    /// <summary>
    /// Update the DataHandler data for the continuous CoP and CoM tracking
    /// </summary>
    /// <param name="posn"></param>
    private void UpdateContinuousData(Vector2 posn)
    {
        // The current color of the target
        Target.posnIndicator curColor = targets[targetIndex].indication;
        float delta = Time.deltaTime;

        // Add the time that the user spent in the current target color
        if (curColor == Target.posnIndicator.GREEN)
        {
            curGreenTime = curGreenTime + delta;
        }
        else if (curColor == Target.posnIndicator.YELLOW)
        {
            curYellowTime = curYellowTime + delta;
        }
        else
        {
            curRedTime = curRedTime + delta;
        }

        // Record the continuous data like CoP and CoM
        if (OnRecordContinuousData != null)
        {
            // note: convert target index to a one-indexed value for data recording
            OnRecordContinuousData(participantId, time, posn,
                curColor, new Vector3(0, 0, 0), targetIndex + 1, curTrial);
        }
    }
}
