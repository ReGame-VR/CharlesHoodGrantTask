using System.Collections;
using System.Collections.Generic;
using System.Linq;
using cakeslice;
using ManusVR.PhysicalInteraction;
using UnityEngine;

namespace ManusVR.PhysicalInteraction
{
    public class InteractableItem : Interactable
    {
        private readonly HashSet<Outline> _outlines = new HashSet<Outline>();
        private bool _assistanceEnabled = false;
        
        private const int RequiredYForce = -1000;
        private const int MaxHorizontalForce = 500;

        protected override void Initialize(Collider[] colliders)
        {
            base.Initialize(colliders);
            
            // Add outlines to all of the colliders
            foreach (Collider child in colliders)
            {
                var outline = child.GetComponent<Outline>();
                if (outline == null && child.GetComponent<MeshRenderer>())
                    outline = child.gameObject.AddComponent<Outline>();
                if (outline != null)
                    _outlines.Add(outline);
            }
        }

        public override void Attach(Rigidbody connectedBody, ObjectGrabber hand)
        {
            base.Attach(connectedBody, hand);
            Rigidbody.isKinematic = false;

            bool freeRotation = false;
            if (Connection is FixedJoint)
                freeRotation = false;   
            else if (GetComponent<FixedJoint>())
                freeRotation = true;
            
            Destroy(Connection);
            if (!freeRotation)
            {
                // Add a fixed joint to the object
                Connection = gameObject.AddComponent<FixedJoint>();

                // Attach the created joint to the hand
                Connection.connectedBody = connectedBody;
                Connection.anchor = Rigidbody.position - connectedBody.position;
            }

            else
            {
                // Add a fixed joint to the object
                ConfigurableJoint rotationJoint = Rigidbody.gameObject.AddComponent<ConfigurableJoint>();
                rotationJoint.configuredInWorldSpace = true;
                Connection = rotationJoint;
                

                rotationJoint.xMotion = ConfigurableJointMotion.Locked;
                rotationJoint.yMotion = ConfigurableJointMotion.Locked;
                rotationJoint.zMotion = ConfigurableJointMotion.Locked;

                // Attach the created joint to the hand
                Connection.anchor = Vector3.zero;
                rotationJoint.autoConfigureConnectedAnchor = true;
                Connection.connectedBody = connectedBody;

            }

            //_grabDistance = Vector3.Distance(transform.position, Hand.HandRigidbody.position);
            Rigidbody.useGravity = GravityWhenGrabbed;

            StartCoroutine(AssistanceCheck(transform.position));

            if (HighlightWhenGrabbed)
                ActivateOutline(true);
        }

        /// <summary>
        /// Only enable the placement assistance when the object traveled a certain distance.
        /// Otherwise it would happen that the grabbed item gets dropped instantly.
        /// </summary>
        /// <param name="grabLocation"></param>
        /// <returns></returns>
        private IEnumerator AssistanceCheck(Vector3 grabLocation)
        {
            _assistanceEnabled = false;
            yield return new WaitUntil((() => Vector3.Distance(grabLocation, transform.position) > DropDistance));
            _assistanceEnabled = true;
        }

        void Update()
        {
            if (Hand == null) return;

            // release this object when it is to far away from the hand
            if (Hand.DropDistance > DropDistance)
                Hand.ReleaseItem(this);

        }

        public override void Dettach(ObjectGrabber hand)
        {
            base.Dettach(hand);
            Rigidbody.useGravity = GravityWhenReleased;
            Rigidbody.isKinematic = KinematicWhenReleased;
            Destroy(Connection);
            if (HighlightWhenGrabbed)
                ActivateOutline(false);
        }

        internal void ActivateOutline(bool active)
        {
            foreach (var outline in _outlines)
                outline.enabled = active;
        }

        /// <summary>
        /// Active the outline when there is collision between the object and fingers
        /// </summary>
        /// <param name="collision"></param>
        protected override void CollisionEnter(Collision collision)
        {
            base.CollisionEnter(collision);
            if (!HighlightOnImpact) return;
            foreach (var detector in _detectors)
            {
                if (detector.GetCollidingObjectsInLayer(Layer.Phalange).ToArray().Length <= 0) continue;
                ActivateOutline(true);
                return;
            }
        }

        /// <summary>
        /// Deactive the outline
        /// </summary>
        /// <param name="collision"></param>
        protected override void CollisionExit(Collision collision)
        {
            base.CollisionExit(collision);

            if (Hand != null || !HighlightOnImpact) return;
            foreach (var detector in _detectors)
                if (detector.GetCollidingObjectsInLayer(Layer.Phalange).ToArray().Length > 0)
                    return;

            ActivateOutline(false);
        }
    }
}



