using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

public class PEMeasuring : MonoBehaviour
{


    // the head mounted display - Camera (head) in [CameraRig] prefab
    public GameObject hmd;

    // The tracked hand of the player
    public GameObject hand;

    // Calibration values
    private float armLen, shoulderHeight, leftReachLocation, rightReachLocation, fullHeight;

    private Vector3 standingPosition;

    private string pid = GlobalControl.Instance.participantID;

    // Use this for initialization
    void Start()
    {
        standingPosition = hmd.transform.position;
        armLen = 0f;
        shoulderHeight = 0f;
        leftReachLocation = 0f;
        rightReachLocation = 0f;
        fullHeight = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        // - - - - FORWARD REACH CALIBRATION - - - -
        if (Input.GetMouseButtonUp(0))
        {
            CaptureArmLengthAndHeight();
        }

        // - - - - SIDE REACH CALIBRATION - - - -
        Vector3 posn3 = hand.transform.position;


        // check to see if it is a new maximum
        if (posn3.x > rightReachLocation)
        {
            rightReachLocation = posn3.x;
        }
        else if (posn3.x < leftReachLocation)
        {
            leftReachLocation = posn3.x;
        }


    }

    // Capture the arm length and height for calibration. Called on mouse up.
    public void CaptureArmLengthAndHeight()
    {
        // Add 10cm (0.1f) to this value to compensate for depth of hmd
        armLen = Mathf.Abs(hand.transform.position.z - hmd.transform.position.z) + 0.1f;
        shoulderHeight = hand.transform.position.y;

        // standingposition is where the HMD is, currently over eyes
        standingPosition = hmd.transform.position;
        // Vertical distance from shoulder to eyes
        float shoulderToEyes = standingPosition.y - shoulderHeight;
        // Eyes to top of head is approximately 80% of vertical distance from shoulder to eyes
        float eyesToTopOfHead = shoulderToEyes * 0.8f;

        // Full height is the height of the eyes plus the distance from eyes to top of head
        fullHeight = standingPosition.y + eyesToTopOfHead;

    }

    public void OnDisable()
    {
        float rightReachDistance = Mathf.Abs(rightReachLocation - standingPosition.x);
        float leftReachDistance = Mathf.Abs(leftReachLocation - standingPosition.x);

        Debug.Log("Arm Length: " + (armLen * 100).ToString() + "cm");
        Debug.Log("Shoulder Height: " + (shoulderHeight * 100).ToString() + "cm");
        Debug.Log("Left Reach Distance: " + (leftReachDistance * 100).ToString() + "cm");
        Debug.Log("Right Reach Distance: " + (rightReachDistance * 100).ToString() + "cm");
        Debug.Log("Full Height: " + (fullHeight * 100).ToString() + "cm");

        // Create a new folder under this participant's name and add the data there.
        Directory.CreateDirectory(@"Data/" + pid);
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/Measuring" + pid + ".csv"))
        {
            // write header
            CsvRow header = new CsvRow();
            header.Add("Arm Length (cm)");
            header.Add("Shoulder Height (cm)");
            header.Add("Left Reach Distance (cm)");
            header.Add("Right Reach Distance (cm)");
            header.Add("Full Height (cm)");
            writer.WriteRow(header);

            // write row
            CsvRow row = new CsvRow();
            row.Add((armLen * 100).ToString());
            row.Add((shoulderHeight * 100).ToString());
            row.Add((leftReachDistance * 100).ToString());
            row.Add((rightReachDistance * 100).ToString());
            row.Add((fullHeight * 100).ToString());
            writer.WriteRow(row);
        }
    }
}
