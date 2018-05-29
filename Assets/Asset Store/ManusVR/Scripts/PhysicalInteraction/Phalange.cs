using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ManusVR.PhysicalInteraction
{
    public class PhalangeData
    {
        public Finger Finger;
        public int Index;
        public device_type_t DeviceTypeT;

        public PhalangeData(Finger finger, int Index, device_type_t DeviceTypeT)
        {
            this.Finger = finger;
            this.Index = Index;
            this.DeviceTypeT = DeviceTypeT;
        }

        public override string ToString()
        {
            return (int) Finger + Index + "";
        }
    }

    public class Phalange : MonoBehaviour
    {
        private Collider[] _colliders;
        public CollisionDetector Detector { get; private set; }

        public PhalangeData PhalangeData { get; set; }
        public Rigidbody Rigidbody { get; private set; }

        public Action<PhalangeData, Collision, CollisionType> CollisionEntered;

        // Use this for initialization
        void Awake ()
        {
            Rigidbody = gameObject.GetComponent<Rigidbody>();
            if (Rigidbody == null)
                Rigidbody = gameObject.AddComponent<Rigidbody>();

            Rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
            Rigidbody.useGravity = false;
            Detector =  gameObject.AddComponent<CollisionDetector>();

            var layer = PhysicsLayer.GetLayer(Layer.Phalange);
            Detector.PhysicsLayers = layer.AllowedCollisions;
            PhysicsManager.Instance.Register(GetComponents<Collider>(), GetComponent<Rigidbody>(), layer);
        }

        void Start()
        {
            _colliders = GetComponents<Collider>();
            
            Detector.CollisionEnter += CollisionEnter;
            Detector.CollisionStay += CollisionStay;
            Detector.CollisionExit += CollisionExit;

            if (PhalangeData.Index == 0 && PhalangeData.Finger == 0)
                Rigidbody.mass = 10;
            else
            {
                Rigidbody.mass = 0.1f;
            }
        }

        private void CollisionEnter(Collision collision)
        {
            CheckCollision(collision, CollisionType.Enter);
        }

        private void CollisionStay(Collision collision)
        {
            CheckCollision(collision, CollisionType.Stay);
        }

        private void CollisionExit(Collision collision)
        {
            CheckCollision(collision, CollisionType.Exit);
        }

        /// <summary>
        /// Happens when this phalange is colliding with an object
        /// </summary>
        /// <param name="collision"></param>
        private void CheckCollision(Collision collision, CollisionType type)
        {
            foreach (var collider in _colliders)
            {
                if (PhysicsManager.Instance.ProcessCollision(collider, collision) && CollisionEntered != null)
                    CollisionEntered(PhalangeData, collision, type);
            } 
        }
    }
}