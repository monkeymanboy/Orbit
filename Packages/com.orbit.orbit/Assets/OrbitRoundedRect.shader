Shader "UI/OrbitRoundedRect"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [HideInInspector] _Color ("Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv0 : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            float4 _ShaderSettings; // x: width, y: height, z: thickness, w: fillMode
            float4 _CornerRadii; // x: BL, y: BR, z: TR, w: TL
            float4 _InnerRectParams; // x: xMin, y: yMin, z: width, w: height

            half4 _ColorsBL, _ColorsTL, _ColorsTR, _ColorsBR;
            half4 _InnerEdgeBL, _InnerEdgeTL, _InnerEdgeTR, _InnerEdgeBR;
            half4 _OuterEdgeBL, _OuterEdgeTL, _OuterEdgeTR, _OuterEdgeBR;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv0;
                o.color = v.color;
                return o;
            }

            float SDFRoundedRect(float2 p, float2 halfSize, float4 r)
            {
                float2 r_picked = (p.x > 0.0) ? ((p.y > 0.0) ? r.zy : r.yy) : ((p.y > 0.0) ? r.ww : r.xw);
                float radius = r_picked.x;

                float2 q = abs(p) - halfSize + radius;
                return min(max(q.x, q.y), 0.0) + length(max(q, 0.0)) - radius;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 dimensions = _ShaderSettings.xy;
                float thickness = _ShaderSettings.z;
                int fillMode = (int)(_ShaderSettings.w + 0.5);

                float2 uvNorm = i.uv;
                if (fillMode == 3) // OuterEdge layout expansion
                {
                    float2 totalSize = dimensions + thickness * 2.0;
                    float2 pixelPos = i.uv * totalSize - thickness;
                    uvNorm = pixelPos / dimensions;
                }

                float2 p = (uvNorm - 0.5) * dimensions;
                float2 halfSize = dimensions * 0.5;

                float dist = SDFRoundedRect(p, halfSize, _CornerRadii);
                float aaWidth = fwidth(dist);

                float alpha = 1.0;
                float x = uvNorm.x;
                float y = uvNorm.y;

                float innerX = (p.x - _InnerRectParams.x) / _InnerRectParams.z;
                float innerY = (p.y - _InnerRectParams.y) / _InnerRectParams.w;
                innerX = saturate(innerX);
                innerY = saturate(innerY);
                
                float4 baseColorInnerPath =
                    _ColorsBL * ((1.0 - x) * (1.0 - y)) +
                    _ColorsBR * (x * (1.0 - y)) +
                    _ColorsTL * ((1.0 - x) * y) +
                    _ColorsTR * (x * y);

                float4 edgeColorInnerPath =
                    _InnerEdgeBL * ((1.0 - innerX) * (1.0 - innerY)) +
                    _InnerEdgeBR * (innerX * (1.0 - innerY)) +
                    _InnerEdgeTL * ((1.0 - innerX) * innerY) +
                    _InnerEdgeTR * (innerX * innerY);

                float4 innerRingColor = baseColorInnerPath * edgeColorInnerPath;

                float4 outerRingColor =
                    _OuterEdgeBL * _ColorsBL * ((1.0 - x) * (1.0 - y)) +
                    _OuterEdgeBR * _ColorsBR * (x * (1.0 - y)) +
                    _OuterEdgeTL * _ColorsTL * ((1.0 - x) * y) +
                    _OuterEdgeTR * _ColorsTR * (x * y);
                
                float edgeLinearFactor = 0.0; //1.0 = inner ring, 0.0 = outer ring

                if (fillMode == 0) // Solid Inner
                {
                    alpha = smoothstep(0.0, -aaWidth, dist);
                    edgeLinearFactor = 1.0; // Locked to Inner Ring
                }
                else if (fillMode == 1) // Solid Outer
                {
                    alpha = smoothstep(-aaWidth, 0.0, dist);
                    edgeLinearFactor = 0.0; // Locked to Outer Ring
                }
                else if (fillMode == 2) // InnerEdge
                {
                    float outAlpha = smoothstep(0.0, -aaWidth, dist);
                    float inAlpha = smoothstep(-thickness - aaWidth, -thickness - aaWidth*2, dist);
                    alpha = outAlpha - inAlpha;

                    // dist goes from -thickness (inner) to 0.0 (outer).
                    // Invert it so -thickness becomes 1.0 (Inner) and 0.0 becomes 0.0 (Outer).
                    edgeLinearFactor = saturate(-dist / thickness);
                }
                else if (fillMode == 3) // OuterEdge
                {
                    float outAlpha = smoothstep(thickness, thickness - aaWidth, dist);
                    float inAlpha = smoothstep(-aaWidth, -aaWidth*2, dist);
                    alpha = outAlpha - inAlpha;

                    // dist goes from 0.0 (inner) to thickness (outer).
                    // Invert it so 0.0 becomes 1.0 (Inner) and thickness becomes 0.0 (Outer).
                    edgeLinearFactor = 1.0 - saturate(dist / thickness);
                }

                if (alpha <= 0.001) discard;

                // Interpolate between the Outer (0.0) and Inner (1.0) formulas correctly
                float4 finalColor = lerp(outerRingColor, innerRingColor, edgeLinearFactor);
                finalColor.a *= alpha;

                return finalColor * i.color;
            }
            ENDCG
        }
    }
}