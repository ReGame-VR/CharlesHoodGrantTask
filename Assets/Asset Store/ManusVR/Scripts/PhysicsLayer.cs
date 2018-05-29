using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ManusVR
{
    public enum Layer
    {
        Object,
        Phalange,
        UnityDefault,
        UI
    }

    [CreateAssetMenu]
    public class PhysicsLayer : ScriptableObject
    {
        public static List<PhysicsLayer> Layers = new List<PhysicsLayer>();
        private static Dictionary<Layer, PhysicsLayer> _layerDictionary = new Dictionary<Layer, PhysicsLayer>();

        public Layer Layer;
        [SerializeField, FormerlySerializedAs("_allowedCollisions")]
        public List<PhysicsLayer> AllowedCollisions;

        public List<PhysicsLayer> DisallowedCollisions
        {
            get
            {
                return Layers.Where(layer => !AllowedCollisions.Contains(layer)).ToList();
            }
        }

        public static PhysicsLayer GetLayer(Layer layer)
        {
            PhysicsLayer physicsLayer;
            return _layerDictionary.TryGetValue(layer, out physicsLayer) ? physicsLayer : null;
        }

        public bool CanCollideWith(PhysicsLayer otherLayer)
        {
            return AllowedCollisions.Contains(otherLayer);
        }

        void OnValidate()
        {
            foreach (var layer in Layers)
            {
                if(layer == null)
                    continue;

                if (layer == this)
                    continue;

                if (AllowedCollisions.Contains(layer))
                {
                    if (!layer.AllowedCollisions.Contains(this))
                    {
                        layer.AllowedCollisions.Add(this);
                        layer.DisallowedCollisions.Remove(this);
                    }
                }
                else
                {
                    if (layer.AllowedCollisions.Contains(this))
                    {
                        layer.AllowedCollisions.Remove(this);
                        layer.DisallowedCollisions.Add(this);
                    }
                }
            }
        }

        void OnEnable()
        {
            if(Layers == null)
                Layers = new List<PhysicsLayer>();
            Layers.Add(this);

            if (_layerDictionary == null)
                _layerDictionary = new Dictionary<Layer, PhysicsLayer>();
            _layerDictionary.Add(Layer, this);
        }

        void OnDisable()
        {
            Layers.Remove(this);
            _layerDictionary.Remove(Layer);
        }
    }
}
