namespace Atlas.Orbit.ComponentProcessors {
    using System.Collections.Generic;
    using TypeSetters;
    using UnityEngine.UI;

    public class GridLayoutGroupProcessor : ComponentProcessor<GridLayoutGroup> {
        public override Dictionary<string, TypeSetter<GridLayoutGroup>> Setters => new() {
            {"Spacing", new Vector2Setter<GridLayoutGroup>((component, value) => component.spacing = value) },
            {"CellSize", new Vector2Setter<GridLayoutGroup>((component, value) => component.cellSize = value) },
            {"StartAxis", new EnumSetter<GridLayoutGroup,GridLayoutGroup.Axis>((component, value) => component.startAxis = value) },
            {"StartCorner", new EnumSetter<GridLayoutGroup,GridLayoutGroup.Corner>((component, value) => component.startCorner = value) },
            {"Constraint", new EnumSetter<GridLayoutGroup,GridLayoutGroup.Constraint>((component, value) => component.constraint = value) },
            {"ConstraintCount", new IntSetter<GridLayoutGroup>((component, value) => component.constraintCount = value) },
        };
    }
}