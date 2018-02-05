using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ManusVR.PhysicalInteraction
{
    public enum CollisionType
    {
        Enter,
        Stay,
        Exit
    }

    public class CollisionDetector : MonoBehaviour
    {
        public Rigidbody Rigidbody { private set; get; }

        private HashSet<Collider> _triggerColliders = new HashSet<Collider>();
        private HashSet<Collider> _collisions = new HashSet<Collider>();
        private Dictionary<GameObject, PhysicsObject> _collidingObjects = new Dictionary<GameObject, PhysicsObject>();
        public bool IsColliding { get; set; }
        public List<PhysicsLayer> PhysicsLayers;
        //private Dictionary<Layer, HashSet<Collider>> _objectsInTriggerByLayer = new Dictionary<Layer, HashSet<Collider>>();

        // Actions
        public Action<Collision> CollisionEnter;
        public Action<Collision> CollisionStay;
        public Action<Collision> CollisionExit;

        public int ObjectsInTrigger { get { return _triggerColliders.Count; } }
        public int Collisions { get { return _collisions.Count; } }

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
        }

        // Use this for initialization
        void Start()
        {
            IsColliding = false;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (_collidingObjects.ContainsKey(collision.gameObject)) return;

            PhysicsObject pObject;
            if (!PhysicsManager.Instance.GetPhysicsObject(collision.gameObject, out pObject)) return;

            if (!PhysicsLayers.Contains(pObject.PhysicsLayer))
                return;

            IsColliding = true;
            StopAllCoroutines();
            StartCoroutine(Entered());
            _collisions.Add(collision.collider);
            _collidingObjects.Add(collision.gameObject, pObject);
            if (CollisionEnter != null)
                CollisionEnter(collision);
        }

        void OnCollisionStay(Collision collision)
        {
            if (!_collisions.Contains(collision.collider)) return;

            IsColliding = true;
            StopAllCoroutines();
            StartCoroutine(Entered());
            if (CollisionStay != null)
                CollisionStay(collision);
        }

        void OnCollisionExit(Collision collision)
        {
            _collisions.Remove(collision.collider);
            if (_collidingObjects.ContainsKey(collision.gameObject))
            {
                _collidingObjects.Remove(collision.gameObject);
            }

            if (CollisionExit != null)
                CollisionExit(collision);
        }

        void OnTriggerEnter(Collider collider)
        {
            PhysicsObject pObject;
            if (!PhysicsManager.Instance.GetPhysicsObject(collider.gameObject, out pObject))
                return;

            if (!PhysicsLayers.Contains(pObject.PhysicsLayer))
                return;

            _triggerColliders.Add(collider);
        }

        void OnTriggerExit(Collider collider)
        {
            if(_triggerColliders.Contains(collider))
                _triggerColliders.Remove(collider);
        }

        public IEnumerable<PhysicsObject> GetCollidingObjectsInLayer(Layer layer)
        {
            return GetCollidingObjectsInLayer(PhysicsLayer.GetLayer(layer));
        }

        public IEnumerable<PhysicsObject> GetCollidingObjectsInLayer(PhysicsLayer layer)
        {
            return _collidingObjects.Values.Where(pObject => pObject.PhysicsLayer == layer);
        }

        /// <summary>
        /// Check if this is a collision that should be registered
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        bool ValidCollision(GameObject gameObject)
        {
            PhysicsObject physicsObject;
            return !PhysicsManager.Instance.GetPhysicsObject(gameObject, out physicsObject) ||
                   PhysicsLayers.Any(layer => layer.CanCollideWith(physicsObject.PhysicsLayer));
        }

        IEnumerator Entered()
        {
            yield return new WaitForSeconds(0.05f);
            IsColliding = false;
        }
    }
}