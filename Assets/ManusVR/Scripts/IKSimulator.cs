/*
   Copyright 2015 Manus VR

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR
{
    [RequireComponent(typeof(HandData))]
    public class IKSimulator : MonoBehaviour
    {
        private IntPtr _session;
        [HideInInspector]
        public float[] handYawOffsets = { 0.0f, 0.0f };
        private bool _useTrackers;

        public HandData HandData;

        [SerializeField]
        private Transform _cameraRig;

        public Transform RootTransform;

        private Transform[] _wristTransforms;
        private Transform[][][] _fingerTransforms;

        public List<Transform> TrackerOffsets;

        public KeyCode CalibrateKey;
        public enum HandAlignmentKeys
        {
            None,
            QW,
            AS
        }
        public HandAlignmentKeys LeftAlignmentKeys = HandAlignmentKeys.QW;
        public HandAlignmentKeys RightAlignmentKeys = HandAlignmentKeys.AS;

        [HideInInspector]
        public Vector3 PostRotLeft = new Vector3(-90, -90, 0);
        [HideInInspector]
        public Vector3 PostRotRight = new Vector3(-90, 90, 0);
        [HideInInspector]
        public Vector3 PostRotThumbLeft = new Vector3(-21.7f, 109.24f, -164.3f);
    
        public Vector3 PostRotThumbRight = new Vector3(212.1f, 82.2f, -28f);

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

        /// <summary>
        /// Constructor which loads the HandModel
        /// </summary>
        void Start()
        {
            if (HandData == null)
                Debug.LogError("HandData can not be null");

            Manus.ManusInit(out _session);
            Manus.ManusSetCoordinateSystem(_session, coor_up_t.COOR_Y_UP, coor_handed_t.COOR_LEFT_HANDED);

            string[] fingers =
            {
                "thumb_0",
                "index_0",
                "middle_0",
                "ring_0",
                "pinky_0"
            };

            if (!_cameraRig)
            {
                if (Camera.main != null)
                    _cameraRig = Camera.main.transform.root;
                Debug.LogWarning("CameraRig reference not set, automatically retrieved root transform of main camera. To avoid usage of wrong transform, consider setting this reference.");
            }
            transform.root.parent = _cameraRig;
            //_cameraRig.parent = transform.root;

            if (!RootTransform)
                RootTransform = transform;

            var manager = GetComponent<TrackingManager>();
            if (manager)
                _useTrackers = manager.trackingToUse == TrackingManager.EUsableTracking.GenericTracker;

            // Rotate the offsets of the TrackerOffset when the user is using controllers
            if (!_useTrackers)
                foreach (var trackerOffset in TrackerOffsets)
                {
                    trackerOffset.localRotation = Quaternion.Euler(90, -180,0);
                    var currentLocalPos = trackerOffset.localPosition;
                    trackerOffset.localPosition = new Vector3(-currentLocalPos.x, -currentLocalPos.y, -currentLocalPos.z);
                }

            _wristTransforms = new Transform[2];
            _wristTransforms[0] = FindDeepChild(RootTransform, "hand_l");
            _wristTransforms[1] = FindDeepChild(RootTransform, "hand_r");

            ik_profile_t profile = new ik_profile_t();
            Manus.ManusGetProfile(_session, out profile);

            // Associate the game transforms with the skeletal model.
            _fingerTransforms = new Transform[2][][];
            _fingerTransforms[0] = new Transform[5][];
            _fingerTransforms[1] = new Transform[5][];
            for (int i = 0; i < 5; i++)
            {
                _fingerTransforms[0][i] = new Transform[5];
                _fingerTransforms[1][i] = new Transform[5];
                for (int j = 1; j < 5; j++)
                {
                    string left = fingers[i] + j.ToString() + "_l";
                    string right = fingers[i] + j.ToString() + "_r";
                    _fingerTransforms[0][i][j] = FindDeepChild(RootTransform, left);
                    _fingerTransforms[1][i][j] = FindDeepChild(RootTransform, right);
                }
            }
        }

        /// <summary>
        /// Updates a skeletal from glove data
        /// </summary>
        void LateUpdate()
        {
            // Update the hands. Most of this data is based directly on the sensors.
            // Hand 0 is the left one, hand 1 the right.
            HandAlignmentKeys[] alignmentKeys = new HandAlignmentKeys[2] { LeftAlignmentKeys, RightAlignmentKeys };
            Vector3[] postRotEuler = new Vector3[2] { PostRotLeft, PostRotRight };
            Vector3[] postRotThumbEuler = new Vector3[2] { PostRotThumbLeft, PostRotThumbRight };

            for (int handNum = 0; handNum < 2; handNum++)
            {
                device_type_t deviceType = (device_type_t)handNum;
                if (!HandData.ValidOutput(deviceType)) continue;

                // Adjust the default orientation of the hand when the CalibrateKey is pressed.
                if (Input.GetKeyDown(CalibrateKey))
                {
                    Debug.Log("Calibrated a hand.");
                    _wristTransforms[handNum].rotation = HandData.GetWristRotation(deviceType) * Quaternion.Euler(postRotEuler[handNum]);
                    float offset = _wristTransforms[handNum].localEulerAngles.z;
                    if (offset > 180)
                        offset -= 360;
                    handYawOffsets[handNum] = -offset;
                }

                // Manually rotate the hands with the HandAlignmentKeys.
                switch (alignmentKeys[handNum])
                {
                    case HandAlignmentKeys.None:
                        break;
                    case HandAlignmentKeys.QW:
                        if (Input.GetKey(KeyCode.Q))
                            handYawOffsets[handNum] -= 1.0f;
                        else if (Input.GetKey(KeyCode.W))
                            handYawOffsets[handNum] += 1.0f;
                        break;
                    case HandAlignmentKeys.AS:
                        if (Input.GetKey(KeyCode.A))
                            handYawOffsets[handNum] -= 1.0f;
                        else if (Input.GetKey(KeyCode.S))
                            handYawOffsets[handNum] += 1.0f;
                        break;
                    default:
                        Debug.LogWarning("The alignment keys for the " + (handNum == 0 ? "left" : "right") + " hand are set to an unknown value.");
                        break;
                }


                // Set the wrist rotation.
                Quaternion postRot = Quaternion.Euler(postRotEuler[handNum]);
                _wristTransforms[handNum].rotation = Quaternion.Euler(0.0f, handYawOffsets[handNum],0.0f ) * HandData.GetWristRotation(deviceType) * postRot;

                // Set the rotation of the fingers.
                for (int joint = 3; joint >= 1; joint--)
                {
                    // Set this joint for all the fingers.
                    for (int finger = 0; finger < 5; finger++)
                        _fingerTransforms[handNum][finger][joint].localRotation = HandData.GetFingerRotation((Finger)finger, deviceType, joint);

                    // Handle joint 1 differently from the rest for the thumb.
                    if (joint == 1)
                    {
                        Quaternion postRotThumb = Quaternion.Euler(postRotThumbEuler[handNum]);
                        _fingerTransforms[handNum][0][joint].rotation = Quaternion.Euler(0.0f, handYawOffsets[handNum], 0.0f) * HandData.GetImuRotation(deviceType) * postRotThumb;
                        //_fingerTransforms[handNum][0][joint].localRotation =  HandData.Instance.GetImuRotation(deviceType) * postRotThumb;
                    }
                } // for loop for the joints
            } // for loop for the hands
        } // void Update()
    }
}
