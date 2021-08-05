Shader "Unlit/ImageRotation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EnlargeX ("EnlargeX", Range(0.0 , 1.0)) = 0
        _EnlargeY ("EnlargeY", Range(0.0 , 1.0)) = 0
        _Rotation ("Rotation", Range(0.0 , 3.14)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            uniform float _Rotation;
            uniform float _EnlargeX;
            uniform float _EnlargeY;

            float2x2 rotate2d(float _angle){
                return float2x2(cos(_angle),-sin(_angle),
                            sin(_angle),cos(_angle));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                i.uv -= float2(.5, .5);

                i.uv = mul(i.uv, rotate2d(_Rotation));
                i.uv = i.uv * float2(_EnlargeX, _EnlargeY);

                i.uv += float2(.5, .5);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }

    }
}
