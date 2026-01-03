using UnityEngine;
using UnityEngine.UI;
using System;

namespace Orbit.Components.Graphic {
    using UnityEngine.Serialization;

    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class RoundedRect : MaskableGraphic {
        public enum FillMode { Inner, Outer, InnerEdge, OuterEdge } //TODO Add Outer mode: Could be used in place of a stencil based mask, instead covering something with a solid color or gradient with a cutout

        [Serializable]
        public struct Corner {
            public static Corner Default = new() { Radius = 32, TriangleCount = 12, Color = Color.white, InnerEdgeColor = Color.white, OuterEdgeColor = Color.white};
            [Min(0)] public float Radius;
            [Min(1)] public int TriangleCount;
            public Color Color;
            public Color OuterEdgeColor;
            public Color InnerEdgeColor;
        }

        private enum RectCorner { BottomLeft, TopLeft, TopRight, BottomRight }

        [Header("Settings")]
        public FillMode fillMode = FillMode.Inner;
        [SerializeField, Min(0)] private float edgeThickness;

        [Header("Corners")]
        [SerializeField] protected Corner topLeft = Corner.Default;
        [SerializeField] protected Corner topRight = Corner.Default;
        [SerializeField] protected Corner bottomLeft = Corner.Default;
        [SerializeField] protected Corner bottomRight = Corner.Default;

        public float EdgeThickness {
            get => edgeThickness;
            set {
                edgeThickness = value;
                SetVerticesDirty();
            }
        }
        
        public Color BottomLeftColor {
            get => bottomLeft.Color;
            set {
                bottomLeft.Color = value;
                SetVerticesDirty();
            }
        }

        public Color TopLeftColor {
            get => topLeft.Color;
            set {
                topLeft.Color = value;
                SetVerticesDirty();
            }
        }

        public Color TopRightColor {
            get => topRight.Color;
            set {
                topRight.Color = value;
                SetVerticesDirty();
            }
        }

        public Color BottomRightColor {
            get => bottomRight.Color;
            set {
                bottomRight.Color = value;
                SetVerticesDirty();
            }
        }
        
        public Color BottomLeftInnerEdgeColor {
            get => bottomLeft.InnerEdgeColor;
            set {
                bottomLeft.InnerEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color TopLeftInnerEdgeColor {
            get => topLeft.InnerEdgeColor;
            set {
                topLeft.InnerEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color TopRightInnerEdgeColor {
            get => topRight.InnerEdgeColor;
            set {
                topRight.InnerEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color BottomRightInnerEdgeColor {
            get => bottomRight.InnerEdgeColor;
            set {
                bottomRight.InnerEdgeColor = value;
                SetVerticesDirty();
            }
        }
        
        public Color BottomLeftOuterEdgeColor {
            get => bottomLeft.OuterEdgeColor;
            set {
                bottomLeft.OuterEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color TopLeftOuterEdgeColor {
            get => topLeft.OuterEdgeColor;
            set {
                topLeft.OuterEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color TopRightOuterEdgeColor {
            get => topRight.OuterEdgeColor;
            set {
                topRight.OuterEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color BottomRightOuterEdgeColor {
            get => bottomRight.OuterEdgeColor;
            set {
                bottomRight.OuterEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color InnerEdgeColor {
            get => topLeft.InnerEdgeColor;
            set {
                topLeft.InnerEdgeColor = value;
                topRight.InnerEdgeColor = value;
                bottomLeft.InnerEdgeColor = value;
                bottomRight.InnerEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public Color OuterEdgeColor {
            get => topLeft.OuterEdgeColor;
            set {
                topLeft.OuterEdgeColor = value;
                topRight.OuterEdgeColor = value;
                bottomLeft.OuterEdgeColor = value;
                bottomRight.OuterEdgeColor = value;
                SetVerticesDirty();
            }
        }

        public float BottomLeftRadius {
            get => bottomLeft.Radius;
            set {
                bottomLeft.Radius = value;
                SetVerticesDirty();
            }
        }

        public float TopLeftRadius {
            get => topLeft.Radius;
            set {
                topLeft.Radius = value;
                SetVerticesDirty();
            }
        }

        public float TopRightRadius {
            get => topRight.Radius;
            set {
                topRight.Radius = value;
                SetVerticesDirty();
            }
        }

        public float BottomRightRadius {
            get => bottomRight.Radius;
            set {
                bottomRight.Radius = value;
                SetVerticesDirty();
            }
        }

        public float CornerRadius {
            get => topLeft.Radius;
            set {
                topLeft.Radius = value;
                topRight.Radius = value;
                bottomLeft.Radius = value;
                bottomRight.Radius = value;
                SetVerticesDirty();
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            Rect rect = rectTransform.rect;
            if(rect.width == 0 || rect.height == 0) return;
            
            AdjustRect(ref rect);

            switch(fillMode) {
                case FillMode.Inner:
                case FillMode.Outer:
                    GenerateSolidMesh(vh, rect);
                    break;
                case FillMode.InnerEdge:
                case FillMode.OuterEdge:
                    if(edgeThickness <= 0) return;
                    GenerateEdgeMesh(vh, rect, edgeThickness, fillMode == FillMode.OuterEdge);
                    break;
            }
        }

        private void GenerateSolidMesh(VertexHelper vh, Rect rect) {
            float maxR = Mathf.Min(rect.width, rect.height) * 0.5f;
            PopulatePoint(vh, rect, rect.center, Color.white); // Index 0

            int vCount = 1;
            AddCornerToFan(vh, rect, RectCorner.BottomLeft, bottomLeft, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.TopLeft, topLeft, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.TopRight, topRight, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.BottomRight, bottomRight, maxR, ref vCount);

            vh.AddTriangle(0, vCount - 1, 1);
        }

        private void GenerateEdgeMesh(VertexHelper vh, Rect rect, float thickness, bool outer) {
            float maxR = Mathf.Min(rect.width, rect.height) * 0.5f;
            int startIdx = vh.currentVertCount;

            float signedThickness = outer ? -thickness : thickness;

            // Generate all vertices for all 4 corners in a continuous sequence
            AddCornerToStrip(vh, rect, RectCorner.BottomLeft, bottomLeft, maxR, thickness, outer);
            //After every strip we may need a supplemental color vertex to smoothly transition inner and outer edge colors if they are different
            if(DoesNeedExtraColorVertex(outer, bottomLeft.InnerEdgeColor, topLeft.InnerEdgeColor, bottomLeft.OuterEdgeColor, topLeft.OuterEdgeColor, out Color innerColor, out Color outerColor)) {
                AddStripPair(vh, rect, new Vector2(rect.xMin,0), new Vector2(rect.xMin + signedThickness,0), innerColor, outerColor, true);
            }
            
            AddCornerToStrip(vh, rect, RectCorner.TopLeft, topLeft, maxR, thickness, outer);
            if (DoesNeedExtraColorVertex(outer, topLeft.InnerEdgeColor, topRight.InnerEdgeColor, topLeft.OuterEdgeColor, topRight.OuterEdgeColor, out innerColor, out outerColor)) {
                AddStripPair(vh, rect, new Vector2(0, rect.yMax), new Vector2(0, rect.yMax - signedThickness), innerColor, outerColor, true);
            }
            
            AddCornerToStrip(vh, rect, RectCorner.TopRight, topRight, maxR, thickness, outer);
            if (DoesNeedExtraColorVertex(outer, topRight.InnerEdgeColor, bottomRight.InnerEdgeColor, topRight.OuterEdgeColor, bottomRight.OuterEdgeColor, out innerColor, out outerColor)) {
                AddStripPair(vh, rect, new Vector2(rect.xMax, 0), new Vector2(rect.xMax - signedThickness, 0), innerColor, outerColor, true);
            }

            AddCornerToStrip(vh, rect, RectCorner.BottomRight, bottomRight, maxR, thickness, outer);
            if (DoesNeedExtraColorVertex(outer, bottomRight.InnerEdgeColor, bottomLeft.InnerEdgeColor, bottomRight.OuterEdgeColor, bottomLeft.OuterEdgeColor, out innerColor, out outerColor)) {
                AddStripPair(vh, rect, new Vector2(0, rect.yMin), new Vector2(0, rect.yMin + signedThickness), innerColor, outerColor, true);
            }

            // Close the loop: Connect the last pair of vertices back to the first pair
            int totalVerts = vh.currentVertCount - startIdx;
            int lastOuter = vh.currentVertCount - 2;
            int lastInner = vh.currentVertCount - 1;
            int firstOuter = startIdx;
            int firstInner = startIdx + 1;

            vh.AddTriangle(lastOuter, firstOuter, firstInner);
            vh.AddTriangle(lastOuter, firstInner, lastInner);
        }

        private bool DoesNeedExtraColorVertex(bool outer, Color innerColorA, Color innerColorB, Color outerColorA, Color outerColorB, out Color innerColor, out Color outerColor) {
            if(innerColorA == innerColorB && outerColorA == outerColorB) {
                innerColor = default;
                outerColor = default;
                return false;
            }
            innerColor = 0.5f * (innerColorA + innerColorB);
            outerColor = 0.5f * (outerColorA + outerColorB);
            if(!outer)
                (innerColor, outerColor) = (outerColor, innerColor);
            return true;
        }

        private void AddCornerToFan(VertexHelper vh, Rect rect, RectCorner corner, Corner p, float maxR, ref int vCount) {
            float r = Mathf.Min(maxR, p.Radius);
            float adjustedR = r - edgeThickness;

            Rect uvRect = rect;
            uvRect.x -= edgeThickness;
            uvRect.y -= edgeThickness;
            uvRect.width += edgeThickness * 2;
            uvRect.height += edgeThickness * 2;
            
            if(adjustedR < 0) {
                //Collapsed into a non rounded corner
                (float startAngle, float endAngle, Vector2 pivot) = GetCornerData(rect, corner, edgeThickness);
                
                PopulatePoint(vh, uvRect, pivot, Color.white);
                vCount++;
                
                if(vCount > 2) {
                    vh.AddTriangle(0, vCount - 2, vCount - 1);
                }
            } else {
                (float startAngle, float endAngle, Vector2 pivot) = GetCornerData(rect, corner, r);

                for(int i = 0; i <= p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
        
                    Vector2 pos = pivot + VectorAtAngle(adjustedR, angle);
        
                    PopulatePoint(vh, uvRect, pos, Color.white);
                    vCount++;
        
                    if(vCount > 2) {
                        vh.AddTriangle(0, vCount - 2, vCount - 1);
                    }
                }
            }
        }

        private void AddCornerToStrip(VertexHelper vh, Rect rect, RectCorner corner, Corner p, float maxR, float thickness, bool outer) {
            float requestedR = Mathf.Min(maxR, p.Radius);
            float outerR = requestedR;
            float innerR = outer ? outerR + thickness : outerR - thickness;

            (float startAngle, float endAngle, Vector2 pivot) = GetCornerData(rect, corner, outerR);
            (float _, float _, Vector2 edgeEndPivot) = GetCornerData(rect, corner, edgeThickness);
    
            if(outer) {
                rect.x -= this.edgeThickness / 2;
                rect.y -= this.edgeThickness / 2;
                rect.width += this.edgeThickness;
                rect.height += this.edgeThickness;
            }

            Color innerColor;
            Color outerColor;
            if(outer) {
                innerColor = p.InnerEdgeColor;
                outerColor = p.OuterEdgeColor;
            } else {
                outerColor = p.InnerEdgeColor;
                innerColor = p.OuterEdgeColor;
            }

            //Both are hard corners (0 radius)
            if (innerR <= 0 && outerR <= 0) {
                AddStripPair(vh, rect, pivot, edgeEndPivot, innerColor, outerColor);
            } 
            //Outer is rounded, but thickness swallows the inner radius
            else if (innerR <= 0) {
                for (int i = 0; i <= p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
                    Vector2 dir = VectorAtAngle(1f, angle);
                    //Outer follows the curve, Inner stays at the pivot point
                    AddStripPair(vh, rect, pivot + dir * outerR, edgeEndPivot, innerColor, outerColor);
                }
            } 
            //Standard rounded strip
            else {
                for (int i = 0; i < p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
                    Vector2 dir = VectorAtAngle(1f, angle);
                    AddStripPair(vh, rect, pivot + dir * outerR, pivot + dir * innerR, innerColor, outerColor);
                }
            }
        }

        private void AddStripPair(VertexHelper vh, Rect rect, Vector2 outer, Vector2 inner, Color innerColor, Color outerColor, bool isExtraColorVertex = false) {
            PopulatePoint(vh, rect, outer, innerColor);
            PopulatePoint(vh, rect, inner, outerColor);
            
            if(vh.currentVertCount > 2) {
                int currOuter = vh.currentVertCount - 2;
                int currInner = vh.currentVertCount - 1;
                int prevOuter = currOuter - 2;
                int prevInner = currInner - 2;
                if(isExtraColorVertex) {
                    vh.AddTriangle(prevInner, currInner, currOuter);
                    vh.AddTriangle(prevInner, currOuter, prevOuter);
                } else {
                    vh.AddTriangle(prevOuter, currOuter, currInner);
                    vh.AddTriangle(prevOuter, currInner, prevInner);
                }
            }
        }

        private (float, float, Vector2) GetCornerData(Rect rect, RectCorner corner, float r) {
            return corner switch {
                RectCorner.BottomLeft => (-90, -180, new Vector2(rect.xMin + r, rect.yMin + r)),
                RectCorner.TopLeft => (180, 90, new Vector2(rect.xMin + r, rect.yMax - r)),
                RectCorner.TopRight => (90, 0, new Vector2(rect.xMax - r, rect.yMax - r)),
                RectCorner.BottomRight => (0, -90, new Vector2(rect.xMax - r, rect.yMin + r)),
                _ => (0, 0, Vector2.zero)
            };
        }


        protected void PopulatePoint(VertexHelper vh, Rect fullRect, Vector2 point, Color colorMultiplier) {
            Vector2 normalizedPoint = Rect.PointToNormalized(fullRect, point);
            Vector2 uv = GetUVForNormalizedPosition(normalizedPoint);
            float x = normalizedPoint.x;
            float y = normalizedPoint.y;
            Color cornerColor =
                bottomLeft.Color * ((1 - x) * (1 - y)) +
                bottomRight.Color * (x * (1 - y)) +
                topLeft.Color * ((1 - x) * y) +
                topRight.Color * (x * y);

            vh.AddVert(point, colorMultiplier * color * cornerColor, uv);
        }

        protected virtual Vector2 GetUVForNormalizedPosition(Vector2 position) {
            return position;
        }

        protected virtual void AdjustRect(ref Rect rect) { }

        private static Vector2 VectorAtAngle(float radius, float angle) {
            float rad = Mathf.Deg2Rad * angle;
            return new Vector2(radius * Mathf.Cos(rad), radius * Mathf.Sin(rad));
        }
    }
}