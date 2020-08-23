﻿Shader "Unlit/EdgeDetector"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _KernelSize("Kernel Size (N)", Int) = 21

		_DeltaX ("Delta X", Float) = 0.01
		_DeltaY ("Delta Y", Float) = 0.01
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		//Gaussian Blur
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag_horizontal

			#include "UnityCG.cginc"

			// Define the constants used in Gaussian calculation.
			static const float TWO_PI = 6.28319;
			static const float E = 2.71828;

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MainTex_TexelSize;
			int	_KernelSize;

			// Two-dimensional Gaussian curve function.
			float gaussian(int x, int y)
			{
				return 1.0;
			}

			float4 frag_horizontal(v2f_img i) : SV_Target
			{
				float3 col = float3(0.0, 0.0, 0.0);
				float kernelSum = 0.0;

				int upper = ((_KernelSize - 1) / 2);
				int lower = -upper;

				for (int x = lower; x <= upper; ++x)
				{
					for (int y = lower; y <= upper; ++y)
					{
						float gauss = gaussian(x, y);
						kernelSum += gauss;

						fixed2 offset = fixed2(_MainTex_TexelSize.x * x, _MainTex_TexelSize.y * y);
						col += gauss * tex2D(_MainTex, i.uv + offset);
					}
				}

				col /= kernelSum;
				return float4(col, 1.0);
			}
			ENDCG
		}

		//Gray Scale
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MainTex_TexelSize;

			float4 frag(v2f_img i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float gray = (col.r * 0.2126) + (col.g *  0.7152) + (col.b * 0.0722);
				col = fixed4(gray, gray, gray, 1);

				return col;
			}
		ENDCG
		}

		//Sobel Edge 
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MainTex_TexelSize;

			float _DeltaX;
			float _DeltaY;
		
			float sobel (sampler2D tex, float2 uv) {
				float2 delta = float2(_DeltaX, _DeltaY);
			
				float4 hr = float4(0, 0, 0, 0);
				float4 vt = float4(0, 0, 0, 0);
			
				hr += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
				//hr += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  0.0;
				hr += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) * -1.0;
				hr += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  2.0;
				//hr += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
				hr += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) * -2.0;
				hr += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) *  1.0;
				//hr += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) *  0.0;
				hr += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
			
				vt += tex2D(tex, (uv + float2(-1.0, -1.0) * delta)) *  1.0;
				vt += tex2D(tex, (uv + float2( 0.0, -1.0) * delta)) *  2.0;
				vt += tex2D(tex, (uv + float2( 1.0, -1.0) * delta)) *  1.0;
				//vt += tex2D(tex, (uv + float2(-1.0,  0.0) * delta)) *  0.0;
				//vt += tex2D(tex, (uv + float2( 0.0,  0.0) * delta)) *  0.0;
				//vt += tex2D(tex, (uv + float2( 1.0,  0.0) * delta)) *  0.0;
				vt += tex2D(tex, (uv + float2(-1.0,  1.0) * delta)) * -1.0;
				vt += tex2D(tex, (uv + float2( 0.0,  1.0) * delta)) * -2.0;
				vt += tex2D(tex, (uv + float2( 1.0,  1.0) * delta)) * -1.0;
			
				return sqrt(hr * hr + vt * vt);
			}
		
			float4 frag (v2f_img IN) : COLOR {
				float s = sobel(_MainTex, IN.uv);
				return float4(s, s, s, 1);
			}
		ENDCG
		}


    }
}
