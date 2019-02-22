// Copyright (c) 2018 ManusVR
using UnityEngine;

namespace Assets.ManusVR.Scripts
{
    public abstract class Wrist : MonoBehaviour, ICollidingCounter {
        private Quaternion _lastRotation = Quaternion.identity;

        public device_type_t DeviceType { get; set; }
        public Hand Hand { get; set; }
        public Rigidbody Rigidbody { get; private set; }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            if (Rigidbody == null)
                Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.centerOfMass = Vector3.zero;    
        }

        public virtual void Start()
        {
        
        }

        /// <summary>
        /// Rotate the wrist towards the given rotation
        /// </summary>
        /// <param name="rotation"></param>
        public virtual void RotateWrist(Quaternion rotation)
        {
            transform.rotation = rotation;
        }

        public virtual int AmountOfCollidingObjects()
        {
            return 0;
        }
    }
}
