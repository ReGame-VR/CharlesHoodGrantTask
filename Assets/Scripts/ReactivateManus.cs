using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// some strange issue causes manus gameobject renderers to deactivate without reason. adding this script to the arm will cause them to reactivate immediately on the second frame
/// </summary>
public class ReactivateManus : MonoBehaviour {

	void Start () {
        StartCoroutine(EnableRenderers());
	}

    IEnumerator EnableRenderers()
    {
        yield return new WaitForEndOfFrame();
        foreach (var skinnedRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            skinnedRenderer.enabled = true;
        }
    }

}
