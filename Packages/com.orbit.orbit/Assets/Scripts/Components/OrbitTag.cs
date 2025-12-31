using System;
using UnityEngine;

namespace Orbit.Components {
    using ComponentProcessors;
    using System.Collections.Generic;

    public class OrbitTag : MonoBehaviour {
        public GameObject ChildrenContainer => childrenContainer == null ? gameObject : childrenContainer;

        [SerializeField] private GameObject childrenContainer;
        [SerializeField] private bool parseChildren = true;
        [SerializeField] private Component[] nonRootComponents;

        public bool ParseChildren => parseChildren;

        /// <summary>
        /// Finds a component for this prefab, first searching through nonRootComponents and then going on to it's own components
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns the found component or null</returns>
        public Component FindComponent(Type type) {
            foreach(Component component in nonRootComponents) {
                if(component == null) throw new Exception($"MarkupPrefab for {name} contains null Non Root Component");
                if(type.IsAssignableFrom(component.GetType()))
                    return component;
            }

            return GetComponent(type);
        }

        public void GetAllComponents(List<Component> components) {
            GetComponents(components);
            components.AddRange(nonRootComponents);
        }

        public void AddComponentIndexes(ComponentProcessor processor, List<(ComponentProcessor, int)> processorIndexList, List<Component> components) {
            bool foundNonRoot = false;
            for(int index = 0;index < nonRootComponents.Length;index++) {
                Component component = nonRootComponents[index];
                if(component == null) throw new Exception($"MarkupPrefab for {name} contains null Non Root Component");
                if(processor.ComponentType.IsAssignableFrom(component.GetType())) {
                    processorIndexList.Add((processor, components.Count-nonRootComponents.Length+index));
                    foundNonRoot = true;
                }
            }

            if(foundNonRoot)
                return;
            Component rootComponent = GetComponent(processor.ComponentType);
            if(rootComponent == null)
                return;
            processorIndexList.Add((processor, components.IndexOf(rootComponent)));
        }
    }
}