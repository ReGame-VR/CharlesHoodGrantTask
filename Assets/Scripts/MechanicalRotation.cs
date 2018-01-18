using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalRotation : MonoBehaviour
{
    /*
    /// The axis the object will rotate around - I.E. the axis that doesn't move.
    [SerializeField]
    private RotationAxis axis;

    /// if the object rotates clockwise (otherwise it rotates counter clockwise)
    [SerializeField]
    private bool clockwise;

    /// the angle that the rotating object is currently on - dictated by time
    private float angle;
    */

    /// the time, in seconds, it takes for one rotation
    [SerializeField]
    private float secondsPerRotation;

    // Rotates the object mechanically
    void Update()
    {
        //This currently rotates around Z axis, update so it can do multiple axes.
        transform.Rotate(Vector3.forward * Time.deltaTime * (360 / secondsPerRotation));
    }

    /// <summary>
    /// An enumeration of the axes.
    /// </summary>
    private enum RotationAxis { x, y, z };
}
