namespace Orbit.SmartImage {
    using global::SmartImage;
    using System;
    using UnityEngine;

    public class OrbitSmartImageManager : MonoBehaviour {
        /*
        private static SmartImageManager instance;
        public static SmartImageManager Instance {
            get {
                if(instance == null) {
                    instance = new GameObject(nameof(OrbitSmartImageManager)).AddComponent<SmartImageManager>();
                }
                return instance;
            }
        }*/
        public static SmartImageManager Instance { get; private set; }

        [SerializeField] private SmartImageManager instance;

        private void Awake() {
            Instance = instance;
        }
    }
    
}