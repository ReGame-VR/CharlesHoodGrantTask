using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ManusVR.Scripts;

public class CalibrateManus : MonoBehaviour {
    // EW - uncertain if this is needed. seems to not exist in current manus sdk. disabling for now.
    // public IKSimulator simulator;

    void Start()
    {
        StartCoroutine(CalibrateManusQuickly());
    }

    IEnumerator CalibrateManusQuickly()
    {
        yield return new WaitForSeconds(0.1f); ;    //Wait a little bit before calibrating
        // simulator.RunUpdateProcedureWithMandatoryCalibration();
    }
}
