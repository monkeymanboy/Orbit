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
            PopulatePoint(vh, rect, rect.center); // Index 0

            int vCount = 1;
            AddCornerToFan(vh, rect, RectCorner.BottomLeft, bottomLeft, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.TopLeft, topLeft, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.TopRight, topRight, maxR, ref vCount);
            AddCornerToFan(vh, rect, RectCorner.BottomRight, bottomRight, maxR, ref vCount);

            vh.AddTriangle(0, vCount - 1, 1);
        }

        private void GenerateEdgeMesh(VertexHelper vh, Rect rect, float thickness, bool outer) {
            float maxRadius = Mathf.Min(rect.width, rect.height) * 0.5f;
            int startIdx = vh.currentVertCount;

            float cornerTransitionOffset = outer ? -0.5f*thickness : 0.25f*thickness;
            
            Rect innerEdgeRect = rect; //For inner edge gradients, goes in by thickness and then consumes the full corners as well
            if(!outer) {
                float innerGradientLeftOffset = -cornerTransitionOffset + 0.5f*Mathf.Min(maxRadius,0.5f * (bottomLeft.Radius + topLeft.Radius));
                float innerGradientRightOffset = -cornerTransitionOffset + 0.5f*Mathf.Min(maxRadius,0.5f * (bottomRight.Radius + topRight.Radius));
                float innerGradientBottomOffset = -cornerTransitionOffset + 0.5f*Mathf.Min(maxRadius,0.5f * (bottomLeft.Radius + bottomRight.Radius));
                float innerGradientTopOffset = -cornerTransitionOffset + 0.5f*Mathf.Min(maxRadius,0.5f * (topLeft.Radius + topRight.Radius));
                innerEdgeRect.x += innerGradientLeftOffset;
                innerEdgeRect.y += innerGradientBottomOffset;
                innerEdgeRect.width -= innerGradientLeftOffset + innerGradientRightOffset;
                innerEdgeRect.height -= innerGradientBottomOffset + innerGradientTopOffset;
            }
            //if(innerEdgeRect.width < 0 || innerEdgeRect.height < 0) //For small rects use base rect (slightly different coloring)
            //    innerEdgeRect = rect;
            // Generate all vertices for all 4 corners in a continuous sequence
            AddCornerToStrip(vh, rect, innerEdgeRect, RectCorner.BottomLeft, bottomLeft, maxRadius, thickness, outer);
            //After every strip we may need a supplemental color vertex to smoothly transition inner and outer edge colors if they are different
            bool hasCornerTransition = DoesNeedExtraColorVertex(bottomLeft.InnerEdgeColor, topLeft.InnerEdgeColor, bottomLeft.OuterEdgeColor, topLeft.OuterEdgeColor);
            if(hasCornerTransition) 
                AddCornerTransition(vh, rect, new Vector2(rect.xMin+cornerTransitionOffset, rect.center.y), bottomLeft, topLeft);
            
            AddCornerToStrip(vh, rect, innerEdgeRect, RectCorner.TopLeft, topLeft, maxRadius, thickness, outer, hasCornerTransition);
            hasCornerTransition = DoesNeedExtraColorVertex(topLeft.InnerEdgeColor, topRight.InnerEdgeColor, topLeft.OuterEdgeColor, topRight.OuterEdgeColor);
            if(hasCornerTransition) 
                AddCornerTransition(vh, rect, new Vector2(rect.center.x, rect.yMax-cornerTransitionOffset), topLeft, topRight);
            
            AddCornerToStrip(vh, rect, innerEdgeRect, RectCorner.TopRight, topRight, maxRadius, thickness, outer, hasCornerTransition);
            hasCornerTransition = DoesNeedExtraColorVertex(topRight.InnerEdgeColor, bottomRight.InnerEdgeColor, topRight.OuterEdgeColor, bottomRight.OuterEdgeColor);
            if(hasCornerTransition) 
                AddCornerTransition(vh, rect, new Vector2(rect.xMax-cornerTransitionOffset, rect.center.y), topRight, bottomRight);

            AddCornerToStrip(vh, rect, innerEdgeRect, RectCorner.BottomRight, bottomRight, maxRadius, thickness, outer, hasCornerTransition);
            hasCornerTransition = DoesNeedExtraColorVertex(bottomRight.InnerEdgeColor, bottomLeft.InnerEdgeColor, bottomRight.OuterEdgeColor, bottomLeft.OuterEdgeColor);
            if(hasCornerTransition) 
                AddCornerTransition(vh, rect, new Vector2(rect.center.x, rect.yMin+cornerTransitionOffset), bottomRight, bottomLeft, true);
            else {
                //If no corner transition at the end need to close out the loop
                int lastOuter = vh.currentVertCount - 2;
                int lastInner = vh.currentVertCount - 1;
                int firstOuter = startIdx;
                int firstInner = startIdx + 1;
                vh.AddTriangle(lastOuter, firstOuter, firstInner);
                vh.AddTriangle(lastOuter, firstInner, lastInner);
            }
        } 
        
        private void AddCornerTransition(VertexHelper vh, Rect rect, Vector2 midPos, Corner c1, Corner c2, bool isLast = false) {
            Color avgColor = (0.5f * (c1.Color + c2.Color)) *
                             (0.5f *
                              ((0.5f * (c1.InnerEdgeColor + c1.OuterEdgeColor)) +
                               (0.5f * (c2.InnerEdgeColor + c2.OuterEdgeColor))));
            PopulatePoint(vh, rect, midPos, avgColor);
            int curr = vh.currentVertCount;
            if(isLast) {
                //close out the loop
                vh.AddTriangle(curr-1, curr-2, curr-3);
                vh.AddTriangle(curr-1, 0, curr-3);
                vh.AddTriangle(curr-1, 0, 1);
                vh.AddTriangle(curr-1, 1, curr-2);
            } else {
                vh.AddTriangle(curr-1, curr-2, curr-3);
                vh.AddTriangle(curr-1, curr-3, curr);
                vh.AddTriangle(curr-1, curr, curr+1);
                vh.AddTriangle(curr-1, curr-2, curr+1);
            }
        }
        private bool DoesNeedExtraColorVertex(Color innerColorA, Color innerColorB, Color outerColorA, Color outerColorB) {
            return innerColorA != innerColorB || outerColorA != outerColorB;
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
                
                PopulatePoint(vh, uvRect, pivot);
                vCount++;
                
                if(vCount > 2) {
                    vh.AddTriangle(0, vCount - 2, vCount - 1);
                }
            } else {
                (float startAngle, float endAngle, Vector2 pivot) = GetCornerData(rect, corner, r);

                for(int i = 0; i <= p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
        
                    Vector2 pos = pivot + VectorAtAngle(adjustedR, angle);
        
                    PopulatePoint(vh, uvRect, pos);
                    vCount++;
        
                    if(vCount > 2) {
                        vh.AddTriangle(0, vCount - 2, vCount - 1);
                    }
                }
            }
        }

        private void AddCornerToStrip(VertexHelper vh, Rect rect, Rect innerRect, RectCorner corner, Corner p, float maxR, float thickness, bool outer, bool hasCornerTransition = false) {
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

            //Both are hard corners (0 radius)
            if (innerR <= 0 && outerR <= 0) {
                AddStripPair(vh, rect, innerRect, pivot, edgeEndPivot, hasCornerTransition);
            } 
            //Outer is rounded, but thickness swallows the inner radius
            else if (innerR <= 0) {
                for (int i = 0; i <= p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
                    Vector2 dir = VectorAtAngle(1f, angle);
                    //Outer follows the curve, Inner stays at the pivot point
                    AddStripPair(vh, rect, innerRect, pivot + dir * outerR, edgeEndPivot, hasCornerTransition && i == 0);
                }
            } 
            //Standard rounded strip
            else {
                for (int i = 0; i < p.TriangleCount; i++) {
                    float angle = Mathf.Lerp(startAngle, endAngle, (float)i / p.TriangleCount);
                    Vector2 dir = VectorAtAngle(1f, angle);
                    AddStripPair(vh, rect, innerRect, pivot + dir * outerR, pivot + dir * innerR, hasCornerTransition && i == 0);
                }
            }
        }

        private void AddStripPair(VertexHelper vh, Rect rect, Rect innerEdgeRect, Vector2 outer, Vector2 inner, bool skipTriangles = false) {
            bool isOuterEdge = fillMode == FillMode.OuterEdge;
            PopulateEdgePoint(vh, rect, innerEdgeRect, outer, isOuterEdge);
            PopulateEdgePoint(vh, rect, innerEdgeRect, inner, !isOuterEdge);
            
            if(vh.currentVertCount > 2) {
                int currOuter = vh.currentVertCount - 2;
                int currInner = vh.currentVertCount - 1;
                int prevOuter = currOuter - 2;
                int prevInner = currInner - 2;
                if(!skipTriangles) {
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


        protected void PopulatePoint(VertexHelper vh, Rect fullRect, Vector2 point) {
            Vector2 normalizedPoint = Rect.PointToNormalized(fullRect, point);
            Vector2 uv = GetUVForNormalizedPosition(normalizedPoint);
            float x = normalizedPoint.x;
            float y = normalizedPoint.y;
            Color cornerColor =
                bottomLeft.Color * ((1 - x) * (1 - y)) +
                bottomRight.Color * (x * (1 - y)) +
                topLeft.Color * ((1 - x) * y) +
                topRight.Color * (x * y);

            vh.AddVert(point, color * cornerColor, uv);
        }
        
        protected void PopulatePoint(VertexHelper vh, Rect fullRect, Vector2 point, Color cornerColor) {
            Vector2 normalizedPoint = Rect.PointToNormalized(fullRect, point);
            Vector2 uv = GetUVForNormalizedPosition(normalizedPoint);

            vh.AddVert(point, color * cornerColor, uv);
        }
        
        protected void PopulateEdgePoint(VertexHelper vh, Rect fullRect, Rect innerEdgeRect, Vector2 point, bool inner) {
            Vector2 normalizedPoint = Rect.PointToNormalized(fullRect, point);
            Vector2 uv = GetUVForNormalizedPosition(normalizedPoint);
            float x = normalizedPoint.x;
            float y = normalizedPoint.y;
            Color topLeftEdgeColor;
            Color topRightEdgeColor;
            Color bottomLeftEdgeColor;
            Color bottomRightEdgeColor;

            Color cornerColor;
            if(inner) {
                topLeftEdgeColor = topLeft.InnerEdgeColor;
                topRightEdgeColor = topRight.InnerEdgeColor;
                bottomLeftEdgeColor = bottomLeft.InnerEdgeColor;
                bottomRightEdgeColor = bottomRight.InnerEdgeColor;
                
                Vector2 innerPoint = Rect.PointToNormalized(innerEdgeRect, point);
                float innerX = innerPoint.x;
                float innerY = innerPoint.y;
                
                cornerColor =
                    bottomLeft.Color * ((1 - x) * (1 - y)) +
                    bottomRight.Color * (x * (1 - y)) +
                    topLeft.Color * ((1 - x) * y) +
                    topRight.Color * (x * y);
                cornerColor *=
                    bottomLeftEdgeColor * ((1 - innerX) * (1 - innerY)) +
                    bottomRightEdgeColor * (innerX * (1 - innerY)) +
                    topLeftEdgeColor * ((1 - innerX) * innerY) +
                    topRightEdgeColor * (innerX * innerY);
            } else {
                topLeftEdgeColor = topLeft.OuterEdgeColor;
                topRightEdgeColor = topRight.OuterEdgeColor;
                bottomLeftEdgeColor = bottomLeft.OuterEdgeColor;
                bottomRightEdgeColor = bottomRight.OuterEdgeColor;
                
                cornerColor =
                    bottomLeftEdgeColor * bottomLeft.Color * ((1 - x) * (1 - y)) +
                    bottomRightEdgeColor * bottomRight.Color * (x * (1 - y)) +
                    topLeftEdgeColor * topLeft.Color * ((1 - x) * y) +
                    topRightEdgeColor * topRight.Color * (x * y);
            }

            vh.AddVert(point, color * cornerColor, uv);
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