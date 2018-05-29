using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ManusVR.PhysicalInteraction
{
    public class HandController : MonoBehaviour
    {
        private const int MotorVelocity = 50;
        private const int MotorForce = 20;
        private readonly HingeJoint[][] _joints = new HingeJoint[5][];
        private Phalange[][] _phalanges;

        private readonly Transform[][] _positionHolders = new Transform[5][];
        private readonly List<CollisionDetector> _thumbDetectors = new List<CollisionDetector>();
        private Coroutine _ghostingCoroutine;

        private JointMotor _motor;
        private Rigidbody[] _rigidbodies;

        private Rigidbody _thumbRigidbody;

        public Action<PhalangeData, Collision, CollisionType> CollisionEntered;
        public device_type_t DeviceType;

        public Transform RootTranform;
        public Transform Target;
        public HandData HandData;

        public Transform Thumb;

        public Phalange[][] Phalanges
        {
            get { return _phalanges; }
        }

        public Rigidbody HandRigidbody { get; private set; }

        public Transform[][] GameTransforms { get; private set; }

        /// <summary>
        ///     How many of the phalanges are currently colliding with a object
        /// </summary>
        public int AmountOfCollidingPhalanges
        {
            get { return (from phalange in _phalanges from p in phalange where p != null select p).Count(p => p.Detector.IsColliding); }
        }

        public float DisconnectDistance
        {
            get { return Vector3.Distance(HandRigidbody.transform.position, Target.position); }
        }

        /// <summary>
        ///     Is the thumb of this hand currently colliding with a object
        /// </summary>
        public bool IsThumbColliding
        {
            get { return _thumbDetectors.Count(detector => detector.IsColliding) > 0; }
        }

        // Use this for initialization
        void Start()
        {
            Application.runInBackground = true;

            string[] fingers =
            {
                "thumb_0",
                "index_0",
                "middle_0",
                "ring_0",
                "pinky_0"
            };

            // Associate the game transforms with the skeletal model.
            GameTransforms = new Transform[5][];
            _phalanges = new Phalange[5][];
            for (var i = 0; i < 5; i++)
            {
                _phalanges[i] = new Phalange[4];
                GameTransforms[i] = new Transform[4];
                for (var j = 1; j < 4; j++)
                {
                    var postfix = DeviceType == device_type_t.GLOVE_LEFT ? "_l" : "_r";
                    var finger = fingers[i] + j + postfix;
                    GameTransforms[i][j] = FindDeepChild(RootTranform, finger);
                }
            }

            // Initializing the hand
            HandRigidbody = RootTranform.gameObject.AddComponent<Rigidbody>();
            var handPhalange = HandRigidbody.gameObject.AddComponent<Phalange>();
            handPhalange.PhalangeData = new PhalangeData(0, 0, DeviceType);
            _phalanges[0][0] = HandRigidbody.GetComponent<Phalange>();

            var phalangeParent = new GameObject("Phalanges");
            phalangeParent.transform.parent = RootTranform;
            ConvertFingersToPhysics(phalangeParent.transform);

            foreach (var phalange in _phalanges)
                foreach (var p in phalange)
                    if (p != null)
                        p.CollisionEntered += PhalangeCollisionEntered;

            _rigidbodies = RootTranform.GetComponentsInChildren<Rigidbody>();
            StartCoroutine(RestrictWristRotation());
        }

        /// <summary>
        ///     Happens when a phalange has collision with an cetrain object
        /// </summary>
        /// <param name="finger">The finger that is colliding</param>
        /// <param name="index">The index of this finger</param>
        /// <param name="collision">All of the collision data</param>
        private void PhalangeCollisionEntered(PhalangeData phalangeData, Collision collision, CollisionType type)
        {
            if (CollisionEntered != null)
                CollisionEntered(phalangeData, collision, type);
        }

        /// <summary>
        ///     Convert the static fingers to physical fingers
        /// </summary>
        private void ConvertFingersToPhysics(Transform phalangeParent)
        {
            for (var index = 0; index < 5; index++)
            {
                _positionHolders[index] = new Transform[4];
                _joints[index] = new HingeJoint[4];
                for (var phalange = 1; phalange < 3; phalange++)
                {
                    // Create a copy of the transform
                    _positionHolders[index][phalange] = CopyTransform(GameTransforms[index][phalange]);
                    _positionHolders[index][phalange].name = "Positionholder" + index + phalange;
                    // Custom localrotation for the index, middel, pink en ring finger
                    if (index < 4 && phalange != 1)
                        _positionHolders[index][phalange].localRotation = Quaternion.Euler(new Vector3(0, 0
                            , 0));
                    else
                        _positionHolders[index][phalange].localRotation = Quaternion.Euler(new Vector3(0, 0
                            , _positionHolders[index][phalange].localRotation.eulerAngles.z));

                    // Unparent the phalangeIndex 
                    GameTransforms[index][phalange].parent = phalangeParent;

                    // When it is a thumb
                    if (index == 0 && phalange == 1)
                    {
                        ConfigureThumb(GameTransforms[index][phalange].gameObject);
                        StartCoroutine(ConnectThumbs(GameTransforms[index][phalange].gameObject));
                        continue;
                    }

                    // Get the rigidbody where it should connect to
                    Rigidbody connectedBody;
                    if (phalange == 1) connectedBody = RootTranform.GetComponent<Rigidbody>();
                    else connectedBody = GameTransforms[index][phalange - 1].GetComponent<Rigidbody>();

                    // Add a hingejoint to the gameobject
                    if (index == 0)
                        _joints[index][phalange] = AddHingeJoint(GameTransforms[index][phalange].gameObject,
                            -RootTranform.right, connectedBody);
                    else
                        _joints[index][phalange] = AddHingeJoint(GameTransforms[index][phalange].gameObject,
                            RootTranform.forward, connectedBody);
                    GameTransforms[index][phalange].gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 100f;

                    ConfigurePhalange(GameTransforms[index][phalange].gameObject, phalange, index);

                    if (index == 0)
                        _thumbDetectors.Add(GameTransforms[index][phalange].GetComponent<CollisionDetector>());

                    if (DeviceType == device_type_t.GLOVE_RIGHT)
                        ChangeJointLimit(_joints[index][phalange], -10, 100);
                    else
                        ChangeJointLimit(_joints[index][phalange], -100, 10);
                }
            }
        }

        private void ConfigureThumb(GameObject thumbGameObject)
        {
            _thumbRigidbody = thumbGameObject.AddComponent<Rigidbody>();
            _thumbRigidbody.mass = 1f;
            _thumbRigidbody.useGravity = false;

            var thumbJoint = thumbGameObject.AddComponent<ConfigurableJoint>();
            thumbJoint.connectedBody = RootTranform.GetComponent<Rigidbody>();

            thumbJoint.xMotion = ConfigurableJointMotion.Locked;
            thumbJoint.yMotion = ConfigurableJointMotion.Locked;
            thumbJoint.zMotion = ConfigurableJointMotion.Locked;
        }

        private IEnumerator ConnectThumbs(GameObject thumbGameObject)
        {
            float initialMass = _thumbRigidbody.mass;
            _thumbRigidbody.mass = 10;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (Quaternion.Angle(_thumbRigidbody.transform.rotation, Thumb.rotation) < 2f &&
                    Vector3.Distance(_thumbRigidbody.transform.position, Thumb.position) < 0.1f)
                    break;
            }

            _thumbRigidbody.mass = initialMass;
            var thumbJoint = thumbGameObject.AddComponent<ConfigurableJoint>();
            var connectedRigidbody = Thumb.gameObject.AddComponent<Rigidbody>();
            connectedRigidbody.isKinematic = true;
            thumbJoint.connectedBody = connectedRigidbody;
            thumbJoint.angularXMotion = ConfigurableJointMotion.Locked;
            thumbJoint.angularYMotion = ConfigurableJointMotion.Locked;
            thumbJoint.angularZMotion = ConfigurableJointMotion.Locked;
        }

        private void ConfigurePhalange(GameObject gameObject, int phalangeIndex, int index)
        {
            var phalange = gameObject.AddComponent<Phalange>();
            _phalanges[index][phalangeIndex] = phalange;
            phalange.PhalangeData = new PhalangeData((Finger)index, phalangeIndex, DeviceType);
        }

        /// <summary>
        ///     Change the jointlimit on a give Hingjoint
        /// </summary>
        /// <param name="joint"></param>
        /// <param name="minLimit"></param>
        /// <param name="maxLimit"></param>
        private void ChangeJointLimit(HingeJoint joint, float minLimit, float maxLimit)
        {
            var limit = new JointLimits();
            limit.min = minLimit;
            limit.max = maxLimit;
            joint.limits = limit;
        }

        /// <summary>
        ///     Add a hingejoint to the gameobject and provide it with the needed settings
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="axis"></param>
        /// <param name="connectedBody"></param>
        /// <returns></returns>
        private HingeJoint AddHingeJoint(GameObject gameObject, Vector3 axis, Rigidbody connectedBody)
        {
            var joint = gameObject.GetComponent<HingeJoint>();
            if (joint != null)
            {
                Debug.LogWarning(gameObject.name + " already has a hingejoint attached to it");
                return joint;
            }
            joint = gameObject.AddComponent<HingeJoint>();

            joint.useLimits = true;
            joint.axis = axis;

            joint.connectedBody = connectedBody;
            joint.useMotor = true;

            return joint;
        }

        private void DetectCollision(bool detect)
        {
            foreach (var rb in _rigidbodies)
                rb.detectCollisions = detect;
        }

        /// <summary>
        ///     Create a copy of the given transform
        /// </summary>
        /// <returns></returns>
        private Transform CopyTransform(Transform transf)
        {
            var newTransform = new GameObject().transform;
            newTransform.parent = transf.parent;
            newTransform.localPosition = transf.localPosition;
            newTransform.localRotation = transf.localRotation;

            return newTransform;
        }

        private IEnumerator RestrictWristRotation()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                float angle = Quaternion.Angle(Target.rotation, RootTranform.rotation);
                if (angle < 0.01f)
                    break;     
            }
            
            var joint = RootTranform.gameObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = Target.GetComponent<Rigidbody>();

            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Limited;
            var limit = new SoftJointLimit();
            limit.limit = -30;
            joint.lowAngularXLimit = limit;
            limit.limit = 30;
            joint.highAngularXLimit = limit;
        }

        /// <summary>
        ///     Finds a deep child in a transform
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

        private void FixedUpdate()
        {
            if (Target != null)
            {
                if (Vector3.Distance(Target.position, HandRigidbody.position) > 0.3f)
                {
                    if (_ghostingCoroutine == null)
                        _ghostingCoroutine = StartCoroutine(StartGhosting());
                }
                var maxPositionDelta = AmountOfCollidingPhalanges == 0 ? 2f : 0.3f;
                var maxRotationDelta = AmountOfCollidingPhalanges == 0 ? 100f : 10f;
                MoveBody(Target, HandRigidbody, maxPositionDelta);
                RotateBody(Target, HandRigidbody, 2, maxRotationDelta, 1000f);
            }
            UpdateFingers();
        }

        private IEnumerator WaitForitemOutOfRange()
        {
            Vector3 targetOffset = -Target.up * 0.1f;
            while (true)
            {
                var hits = Physics.OverlapSphere(Target.position + targetOffset, 0.13f);
                if (DisconnectDistance < 0.1f && !hits.Any(raycastHit => raycastHit.transform.GetComponent<InteractableItem>()))
                    break;
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator StartGhosting()
        {
            DetectCollision(false);
            yield return StartCoroutine(WaitForitemOutOfRange());
            DetectCollision(true);
            _ghostingCoroutine = null;
        }

        /// <summary>
        ///     Move the given rigidbody to the target location
        /// </summary>
        /// <param name="target"></param>
        /// <param name="body"></param>
        /// <param name="maxPositionDelta"></param>
        private void MoveBody(Transform target, Rigidbody body, float maxPositionDelta)
        {
            var posDelta = target.position - body.position;
            var velocityTarget = Vector3.ClampMagnitude(posDelta / Time.fixedDeltaTime, 5);
            body.velocity = Vector3.MoveTowards(body.velocity, velocityTarget, maxPositionDelta);
        }

        /// <summary>
        ///     rotate the given rigidbody to the target rotation
        /// </summary>
        /// <param name="target"></param>
        /// <param name="body"></param>
        /// <param name="velocityMultiplier"></param>
        /// <param name="maxRotationDelta"></param>
        private void RotateBody(Transform target, Rigidbody body, float velocityMultiplier, float maxRotationDelta, float maxAngularVelocity)
        {
            body.maxAngularVelocity = maxAngularVelocity;
            var rotDelta = target.rotation * Quaternion.Inverse(body.transform.rotation);
            float angle;
            Vector3 axis;

            rotDelta.ToAngleAxis(out angle, out axis);
            if (angle > 180) angle -= 360;
            var angularTarget = angle * axis * velocityMultiplier;

            if (angularTarget.magnitude > 0.001f)
                body.angularVelocity = Vector3.MoveTowards(body.angularVelocity, angularTarget, maxRotationDelta);
        }

        /// <summary>
        ///     Get the angle between two given vectors with a positive and negative offset
        /// </summary>
        /// <param name="n">Normal</param>
        /// <param name="from">From</param>
        /// <param name="to">To</param>
        /// <param name="offset"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private float OffsetAngleBetween(Vector3 n, Vector3 from, Vector3 to, float offset, out int dir)
        {
            var offsetAngle1 = Vector3.Angle(Quaternion.AngleAxis(offset + 0.01f, n) * from, to);
            var offsetAngle2 = Vector3.Angle(Quaternion.AngleAxis(offset - 0.01f, n) * from, to);
            dir = offsetAngle1 > offsetAngle2 ? 1 : -1;
            return Vector3.Angle(Quaternion.AngleAxis(offset, n) * from, to);
        }

        private void UpdateFingers()
        {
            MoveFinger(Finger.thumb);
            MoveFinger(Finger.index);
            MoveFinger(Finger.middle);
            MoveFinger(Finger.ring);
            MoveFinger(Finger.pink);
            var maxRotationDelta = !IsThumbColliding ? 200f : 20f;
            RotateBody(Thumb, _thumbRigidbody, 5, maxRotationDelta, 7);
        }

        /// <summary>
        ///     Change the motor on the hinge joint
        /// </summary>
        /// <param name="finger"></param>
        private void MoveFinger(Finger finger)
        {
            var index = (int) finger;
            for (var phalange = 1; phalange < 4; phalange++)
            {
                if (index == 0 && phalange == 1)
                    continue;
                if (phalange > 2)
                    GameTransforms[index][phalange].localRotation =
                        HandData.GetFingerRotation(finger, DeviceType, phalange);
                else
                   MoveFingerWithPhysics(finger, index, phalange);
            }
        }

        private void MoveFingerWithPhysics(Finger finger, int index, int phalange)
        {
            var fingerEulerRot = HandData.GetFingerRotation(finger, DeviceType, phalange).eulerAngles;
            int dir;
            float angle;
            if (index == 0)
            {
                angle = OffsetAngleBetween(_positionHolders[index][phalange].forward,
                    _positionHolders[index][phalange].up
                    , GameTransforms[index][phalange].up, fingerEulerRot.z, out dir);

                // rotate the direction if this is the left glove              
                if (DeviceType == device_type_t.GLOVE_LEFT) dir = dir * -1;
            }
            else
            {
                // Check what way the phalangeIndex should move towards
                var q1 = Quaternion.Euler(0, fingerEulerRot.y + 0.01f, 0);
                var q2 = Quaternion.Euler(0, fingerEulerRot.y - 0.01f, 0);

                var angle1 = Quaternion.Angle(_positionHolders[index][phalange].rotation * q1,
                    GameTransforms[index][phalange].rotation);
                var angle2 = Quaternion.Angle(_positionHolders[index][phalange].rotation * q2,
                    GameTransforms[index][phalange].rotation);

                dir = angle1 > angle2 ? 1 : -1;

                var sensorRot = Quaternion.Euler(0, fingerEulerRot.y, 0);
                angle = Quaternion.Angle(_positionHolders[index][phalange].rotation * sensorRot,
                    GameTransforms[index][phalange].rotation);
            }

            // set the velocity and force to the motor of the hinge joint
            var joint = _joints[index][phalange];
            var targetVelocity = angle;
            if (angle < 3)
                targetVelocity = Mathf.Sqrt(targetVelocity);
            targetVelocity *= dir * MotorVelocity;
            
            targetVelocity = _phalanges[index][phalange].Detector.IsColliding ? Mathf.Clamp(targetVelocity, -80, 80) : Mathf.Clamp(targetVelocity, -800, 800);
            _motor.targetVelocity = targetVelocity;

            _motor.force = MotorForce;
            joint.motor = _motor;
        }
    }
}