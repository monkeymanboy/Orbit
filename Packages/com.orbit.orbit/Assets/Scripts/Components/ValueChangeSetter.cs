﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace Orbit.Components {
    public class ValueChangeSetter : MonoBehaviour {
        public event Action OnObjectDestroyed;

        private void OnDestroy() {
            OnObjectDestroyed?.Invoke();
        }

        public void NotifyDestroyed() {
            OnObjectDestroyed?.Invoke();
        }
    }
}
