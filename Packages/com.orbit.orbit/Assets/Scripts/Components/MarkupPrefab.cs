using System;
using UnityEngine;

namespace Orbit.Components {
    using ComponentProcessors;
    using Parser;

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
        
        /// <summary>
        /// Processes every instance of an external component for the given type, or if there are no external, just processes one component on the base object
        /// </summary>
        /// <returns>Whether or not anything was processed</returns>
        public bool ProcessComponentType(ComponentProcessor processor, TagParameters parameters) {
            bool foundNonRoot = false;
            foreach(Component component in nonRootComponents) { //TODO(PERFORMANCE): Might be worthwhile to build a dictionary cache thats shared between instances of the same prefab to speed this up.
                if(component == null) throw new Exception($"MarkupPrefab for {name} contains null Non Root Component");
                if(processor.ComponentType.IsAssignableFrom(component.GetType())) {
                    processor.Process(component, parameters);
                    foundNonRoot = true;
                }
            }

            if(foundNonRoot)
                return true;
            Component rootComponent = GetComponent(processor.ComponentType);
            if(rootComponent == null)
                return false;
            processor.Process(rootComponent, parameters);
            return true;
        }
    }
}
