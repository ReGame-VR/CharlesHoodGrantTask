using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ManusVR;

public class CalibrateManus : MonoBehaviour {

    public IKSimulator simulator;

    void Start()
    {
        StartCoroutine(CalibrateManusQuickly());
    }

    IEnumerator CalibrateManusQuickly()
    {
        yield return new WaitForSeconds(0.1f); ;    //Wait a little bit before calibrating
        simulator.RunUpdateProcedureWithMandatoryCalibration();
    }
}
