Shader "Unlit/OrigamiShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SideTex ("Side Texture", 2D) = "white" {}

        _CtrlPointRadius ("CtrlPoint Radius", Range(0, 2)) = 1

        _CtrlInterpolation ("Ctrl Interpolation", Range(0, 1)) = 0

        [Toggle(SHOW_SIDE_TEX)]
        _ShowSideTex("Show Side Texture", Int) = 0
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }

        LOD 100

        Pass
        {
            Cull Off 
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            uniform sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform sampler2D _SideTex;
            uniform int _ShowSideTex;

            uniform float4 _ControlPoints[3];
            uniform float4 _OriControlPoints[3];
            float _CtrlPointRadius;
            float _CtrlInterpolation;

            int FindClosestCtrlPoint(float4 vertex) {
                float shortestDist = 10000;
                int m_index = 0;

                for (int i = 0; i < 3; i++) {
                    float dist = distance(vertex, _OriControlPoints[i]);

                    if (dist < shortestDist) {
                        m_index = i;
                        shortestDist = dist;
                    }
                }

                return m_index;
            }

            float4 GetCtrlPointEffectVertex(float4 vertex) {

                int closestIndex = FindClosestCtrlPoint(vertex);

                float weight = (_CtrlPointRadius) - abs(vertex.z - _ControlPoints[closestIndex].z);

                float w_strength = clamp(weight, 0, weight) / _CtrlPointRadius;

                int stepValue = (w_strength > 0) ? 1 : 0;

                return vertex + lerp(_ControlPoints[closestIndex] * stepValue, _ControlPoints[closestIndex] * (w_strength), 1 - _CtrlInterpolation);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos( v.vertex );
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                if (_ShowSideTex == 0) {
                    o.color = 0;
                }
                else {
                    o.color = v.color;
                }

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

               if (i.color.r > 0.1) {
                    col = tex2D(_SideTex, i.uv);                
               }

                return col;
            }
            ENDCG
        }
    }
}
