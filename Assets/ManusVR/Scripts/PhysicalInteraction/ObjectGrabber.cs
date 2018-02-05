using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ManusVR;
using UnityEngine;

namespace ManusVR.PhysicalInteraction
{
    /// <summary>
    ///     Use this script to grab objects with the manus gloves
    /// </summary>
    public class ObjectGrabber : MonoBehaviour
    {
        public device_type_t DeviceType;                            // The deviceType that belongs to the grabber
        public TriggerBinder TriggerBinder;                         // The triggerbinder on the hand
        public Action<GameObject, device_type_t> OnItemGrabbed;
        public Interactable GrabbedItem { get { return _grabbedItem; } }
        public Rigidbody HandRigidbody { get { return _handController.HandRigidbody; } }
        public HandData HandData;

        private ThrowHandler _throwHandler;
        private double _averageOnGrab = 0;

        // Variables for releasing objects
        private const int _ReleaseTrigger = -45;
        private const double _averageGrabOffset = 0.05f;
        private double _oldOpeningSpeed;

        private Interactable _grabbedItem;
        private HandController _handController;

        public float DropDistance { get { return _handController.DisconnectDistance; }}

        private void Start()
        {
            var controllers = GetComponents<HandController>();
            foreach (var controller in controllers)
                if (controller.DeviceType == DeviceType)
                    _handController = controller;

            _throwHandler = gameObject.GetComponentsInChildren<ThrowHandler>()
                .FirstOrDefault(handler => handler.DeviceType == DeviceType);

            if (_throwHandler == null)
            {
                _throwHandler = gameObject.AddComponent<ThrowHandler>();
                _throwHandler.DeviceType = DeviceType;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            UpdateGrabObjects();
            UpdateReleaseObjects();
        }

        /// <summary>
        /// Try to grab items
        /// </summary>
        private void UpdateGrabObjects()
        {
            if (_grabbedItem != null) return;

            // Always try to grab a item when the user is making a fist
            if (HandData.HandClosed(DeviceType))
                foreach (var rb in TriggerBinder.CollidingInteractables)
                    GrabItem(rb);

            // Grab when enough phalanges are colliding
            if (_handController.IsThumbColliding && _handController.AmountOfCollidingPhalanges > 1
                     && HandData.GetCloseValue(DeviceType) != CloseValue.Open)
            {
                foreach (var rb in TriggerBinder.CollidingInteractables)
                    GrabItem(rb);
            }

        }

        /// <summary>
        /// Try to release items
        /// </summary>
        private void UpdateReleaseObjects()
        {

            var openingspeed = UpdateOpeningspeed();
            if (_grabbedItem == null)
                return;
            if (_grabbedItem != null && _grabbedItem.Hand != this)
                return;

            // Release item when the hand is completely open
            if (HandData.HandOpened(DeviceType))
            {
                ReleaseItem(_grabbedItem);
                //Debug.Log("Released because the hand was fully open");
            }

            // Release the object when the openingspeed is high enough
            if (openingspeed < _ReleaseTrigger)
            {
                ReleaseItem(_grabbedItem);
                //Debug.Log("Release because the openingspeed was high enough");
            }

            //Release object if 0 phalanges are inside of the object
            if (_grabbedItem != null && _grabbedItem.ReleaseWithPhalanges && _grabbedItem.TotalObjectsInTriggers <= 1)
            {
                ReleaseItem(_grabbedItem);
                //Debug.Log("Release because all of the phalanges are out of the object");
            }

            if (HandData.Average(DeviceType) < _averageOnGrab)
            {
                ReleaseItem(_grabbedItem);
                //Debug.Log("Release this item " + HandData.Instance.Average(DeviceType) + " - " + _averageOnGrab);
            }
        }

        /// <summary>
        /// Gets the current openingspeed of the hand,
        /// A negative value means that the hand is opening
        /// </summary>
        /// <returns></returns>
        private float UpdateOpeningspeed()
        {
            float difference = (float)HandData.Average(DeviceType) - (float)_oldOpeningSpeed;
            float openingSpeed = difference * 100000 * Time.deltaTime;
            _oldOpeningSpeed = (float)HandData.Average(DeviceType);
            return openingSpeed;
        }

        /// <summary>
        /// Release the grabbed item and turn collision with the object back on
        /// </summary>
        /// <returns></returns>
        private IEnumerator Release(Interactable interactable)
        {
            if (interactable == null) yield break;

            if (interactable == _grabbedItem)
                _grabbedItem = null;

            interactable.Dettach(this);
            _throwHandler.OnObjectRelease(interactable.Rigidbody);

            yield return new WaitUntil(() => interactable.TotalObjectsInTriggers <= 0);
            IgnoreCollision(interactable.Rigidbody, false);
        }

        public void ReleaseItem(Interactable interactable)
        {
            StartCoroutine(Release(interactable));
        }

        /// <summary>
        /// Ignore collision between the grabbed item and the hand
        /// </summary>
        /// <param name="rb"></param>
        /// <param name="ignore"></param>
        public void IgnoreCollision(Rigidbody rb, bool ignore)
        {
            foreach (var phalange in _handController.Phalanges)
            {
                foreach (var p in phalange)
                {
                    if (p == null) continue;
                    var fingerCollider = p.GetComponent<Collider>();                  
                    if (fingerCollider != null)
                    {
                        PhysicsObject physicsObject = null;
                        if (PhysicsManager.Instance.GetPhysicsObject(rb.gameObject, out physicsObject))
                            foreach (var c in physicsObject.Colliders)
                                Physics.IgnoreCollision(fingerCollider, c, ignore);
                    }
                }

            }
        }

        /// <summary>
        /// Try to grab the given rigidbody
        /// </summary>
        /// <param name="interactable"></param>
        private void GrabItem(Interactable interactable)
        {
            if (_grabbedItem != null) return;
            PhysicsObject physicsObject = null;
            if (!PhysicsManager.Instance.GetPhysicsObject(interactable.gameObject, out physicsObject) || !interactable.IsGrabbable)
                return;

            _grabbedItem = interactable;
            _grabbedItem.Attach(_handController.HandRigidbody, this);

            _averageOnGrab = HandData.Average(DeviceType) - _averageGrabOffset;

            // Ignore collision between the grabbed object and the grabbing hand
            IgnoreCollision(physicsObject.Rigidbody, true);

            if (OnItemGrabbed != null)
                OnItemGrabbed(_grabbedItem.gameObject, DeviceType);
            VibrateHand();
        }

        /// <summary>
        /// Vibrate the glove
        /// </summary>
        private void VibrateHand()
        {
            Manus.ManusSetVibration(HandData.Session, DeviceType, 0.7f, 150);
        }
    }
}

