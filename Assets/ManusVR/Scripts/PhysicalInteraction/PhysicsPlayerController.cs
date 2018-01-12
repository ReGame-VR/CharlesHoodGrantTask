using System;
using System.Collections;
using System.Collections.Generic;
using ManusVR;
using ManusVR.PhysicalInteraction;
using UnityEngine;

public class PhysicsPlayerController : MonoBehaviour {

    public IKSimulator IKSimulatorTarget;

    public Action<PhalangeData, Collision, CollisionType> OnHandCollision;
    public Action<PhysicsObject, device_type_t> OnGrabbedItem;
    [Header("Handcontrollers")] public List<HandController> HandControllers = new List<HandController>();

    // Use this for initialization
    void Awake () {
        if (IKSimulatorTarget == null)
            Debug.LogError("IKSimulator can not be null");

	    Initialize(device_type_t.GLOVE_LEFT, FindDeepChild(transform, "hand_l"), FindDeepChild(IKSimulatorTarget.TrackerOffsets[0], "hand_l"), FindDeepChild(IKSimulatorTarget.TrackerOffsets[0], "thumb_01_l"));
	    Initialize(device_type_t.GLOVE_RIGHT, FindDeepChild(transform, "hand_r"), FindDeepChild(IKSimulatorTarget.TrackerOffsets[1], "hand_r"), FindDeepChild(IKSimulatorTarget.TrackerOffsets[1], "thumb_01_r"));
    }

    private void Initialize(device_type_t deviceType, Transform root, Transform target, Transform thumbTransform)
    {
        GameObject handGameobject;
        if (deviceType == device_type_t.GLOVE_LEFT)
            handGameobject = IKSimulatorTarget.TrackerOffsets[0].gameObject;
        else
            handGameobject = IKSimulatorTarget.TrackerOffsets[1].gameObject;

        // Initialize visual body
        Rigidbody rb = target.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        SphereCollider collider = target.gameObject.AddComponent<SphereCollider>();
        collider.center = new Vector3(0, -0.12f, -0.03f);
        collider.radius = 0.06f;
        collider.isTrigger = true;
        target.gameObject.AddComponent<TriggerBinder>();

        // Initialize the controller
        var controller = handGameobject.AddComponent<HandController>();
        HandControllers.Add(controller);

        controller.DeviceType = deviceType;
        controller.RootTranform = root;
        controller.Target = target;
        controller.Thumb = thumbTransform;
        controller.CollisionEntered += HandCollisionEntered;
        controller.HandData = IKSimulatorTarget.HandData;

        var grabber = handGameobject.AddComponent<ObjectGrabber>();
        grabber.DeviceType = deviceType;
        grabber.TriggerBinder = target.GetComponent<TriggerBinder>();
        grabber.HandData = IKSimulatorTarget.HandData;

        grabber.OnItemGrabbed += GrabbedItem;
    }

    private void HandCollisionEntered(PhalangeData data, Collision collision, CollisionType type)
    {
        if (OnHandCollision != null)
            OnHandCollision(data, collision, type);
    }

    private void GrabbedItem(GameObject gameObject, device_type_t type)
    {
        if (OnGrabbedItem == null) return;
        PhysicsObject physicsObject = null;
        PhysicsManager.Instance.GetPhysicsObject(gameObject, out physicsObject);
        OnGrabbedItem(physicsObject, type);
    }

    /// <summary>
    /// Finds a deep child in a transform
    /// </summary>
    /// <param name="aParent">Transform to be searched</param>
    /// <param name="aName">Name of the (grand)child to be found</param>
    /// <returns></returns>
    private static Transform FindDeepChild(Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = FindDeepChild(child, aName);
            if (result != null)
                return result;
        }
        return null;
    }
}
