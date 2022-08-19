using System;
using UnityEngine;

namespace Atlas.Orbit.Components {
    public class MarkupPrefab : MonoBehaviour{
        public GameObject ChildrenContainer => childrenContainer == null ? gameObject : childrenContainer;

        [SerializeField]
        private GameObject childrenContainer;
        [SerializeField]
        private bool parseChildren = true;
        [SerializeField]
        private Component[] nonRootComponents;

        public bool ParseChildren => parseChildren;

        /// <summary>
        /// Finds a component for this prefab, first searching through nonRootComponents and then going on to it's own components
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns the found component or null</returns>
        public Component FindComponent(Type type) {
            foreach(Component component in nonRootComponents) { //TODO(PERFORMANCE): Might be worthwhile to build a dictionary cache thats shared between instances of the same prefab to speed this up.
                if(component == null) throw new Exception($"MarkupPrefab for {name} contains null Non Root Component");
                if(type.IsAssignableFrom(component.GetType()))
                    return component;
            }
            return GetComponent(type);
        }
    }
}
