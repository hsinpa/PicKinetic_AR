Shader "Unlit/OrigamiShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SideTex ("Side Texture", 2D) = "white" {}

        _CtrlPointRadius ("CtrlPoint Radius", Range(0, 50)) = 1

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

            float4 GetCtrlPointEffectVertex(float4 vertex) {

                float4 changeVector = float4(0,0,0,0);
                for (int i = 0; i < 3; i++) {

                    float weight = (_CtrlPointRadius) - abs(vertex.z - _OriControlPoints[i].z);

                    float w_strength = clamp(weight, 0.0, _CtrlPointRadius) / (_CtrlPointRadius + 0.0000001);

                    int stepValue = step(0.0, w_strength);

                    float4 dist = (_ControlPoints[i] - _OriControlPoints[i]);

                    changeVector += dist * w_strength;
                }

                return vertex + (changeVector);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(GetCtrlPointEffectVertex( v.vertex) );
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
