using System.Collections;
using System.Collections.Generic;
using ManusVR;
using ManusVR.PhysicalInteraction;
using UnityEngine;

public class ArmsVisuals : MonoBehaviour {

    [Header("Visual settings")]
    public SkinnedMeshRenderer LeftVisualHand;
    public SkinnedMeshRenderer RightVisualHand;
    public SkinnedMeshRenderer LeftVisualUnderarm;
    public SkinnedMeshRenderer RightVisualUnderarm;

    public Transform LeftUnderarm;
    public Transform RightUnderarm;
    public Transform LeftUnderarmRot;
    public Transform RightUnderarmRot;
    private PhysicsPlayerController _playerController;

    [Range(0, 1)]
    public float MaxOpacity = 0.7f;

    private Renderer _leftHandRenderer;
    private Renderer _rightHandRenderer;
    private Renderer _leftUnderarm;
    private Renderer _rightUnderarm;
    private void Start()
    {
        _leftHandRenderer = LeftVisualHand.GetComponent<Renderer>();
        _rightHandRenderer = RightVisualHand.GetComponent<Renderer>();
        _leftUnderarm = LeftVisualUnderarm.GetComponent<Renderer>();
        _rightUnderarm = RightVisualUnderarm.GetComponent<Renderer>();
        _playerController = GetComponent<PhysicsPlayerController>();
    }

    float Distance(HandController controller)
    {
        Vector3 pos1 = controller.Target.position;
        Vector3 pos2 = controller.HandRigidbody.position;
        return Vector3.Distance(pos1, pos2);
    }
    // Update is called once per frame
    void Update ()
	{
        ChangeArmsOpacity(Distance(_playerController.HandControllers[0]), _leftHandRenderer, _leftUnderarm);
	    ChangeArmsOpacity(Distance(_playerController.HandControllers[1]), _rightHandRenderer, _rightUnderarm);

	    LeftUnderarm.position = _playerController.HandControllers[0].RootTranform.position;
	    RightUnderarm.position = _playerController.HandControllers[1].RootTranform.position;


        LeftUnderarm.rotation = LeftUnderarmRot.rotation;
	    RightUnderarm.rotation = RightUnderarmRot.rotation;
	}

    void ChangeArmsOpacity(float distance, Renderer handRenderer, Renderer armRenderer)
    {
        distance *= Mathf.Round(distance * 5000f) / 100f;
        distance -= 0.2f;
        handRenderer.enabled = !(distance <= 0.04f);

        Color handColor = handRenderer.material.color;
        Color armColor = armRenderer.material.color;

        handColor.a = Mathf.Clamp(distance, 0, MaxOpacity);
        armColor.a = Mathf.Clamp(distance, 0, MaxOpacity);

        handRenderer.material.color = handColor;
        armRenderer.material.color = armColor;
    }
}
