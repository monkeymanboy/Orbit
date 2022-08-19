using System.Collections.Generic;
using UnityEngine;

namespace Atlas.Orbit.ComponentProcessors {
    using TypeSetters;

    public class TransformProcessor : ComponentProcessor<Transform> {
        public override Dictionary<string, TypeSetter<Transform>> Setters => new() {
            {"Rotation", new Vector3Setter<Transform>((component, value) => component.localEulerAngles = value) },
            {"RotationX", new FloatSetter<Transform>((component, value) => component.localEulerAngles = new Vector3(value, component.localEulerAngles.y, component.localEulerAngles.z)) },
            {"RotationY", new FloatSetter<Transform>((component, value) => component.localEulerAngles = new Vector3(component.localEulerAngles.x, value, component.localEulerAngles.z)) },
            {"RotationZ", new FloatSetter<Transform>((component, value) => component.localEulerAngles = new Vector3(component.localEulerAngles.x, component.localEulerAngles.y, value)) },
            {"Position", new Vector3Setter<Transform>((component, value) => component.localPosition = value) },
            {"PositionX", new FloatSetter<Transform>((component, value) => component.localPosition = new Vector3(value, component.localPosition.y, component.localPosition.z)) },
            {"PositionY", new FloatSetter<Transform>((component, value) => component.localPosition = new Vector3(component.localPosition.x, value, component.localPosition.z)) },
            {"PositionZ", new FloatSetter<Transform>((component, value) => component.localPosition = new Vector3(component.localPosition.x, component.localPosition.y, value)) },
            {"Scale", new FloatSetter<Transform>((component, value) => component.localPosition = new Vector3(value, value, value)) },
            {"Active", new BoolSetter<Transform>((component, value) => component.gameObject.SetActive(value)) }
        };
    }
}
