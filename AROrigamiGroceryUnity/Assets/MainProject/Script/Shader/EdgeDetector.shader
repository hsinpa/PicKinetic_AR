Shader "Unlit/EdgeDetector"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlendTex("Blending Tex", 2D) = "white" {}
        _KernelSize("Kernel Size (N)", Int) = 21
		_Threshold("Line threshold", Float) = 0.1
		_BlendTransition("Blending", Range(0, 1)) = 1

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
            #pragma target 3.0

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
            #pragma target 3.0

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
            #pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float2 _MainTex_TexelSize;

			float _DeltaX;
			float _DeltaY;
			float _Threshold;

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
		//Sharp
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;
			float _Threshold;

			float Sharp (sampler2D tex, float2 uv) {
			
				float4 sum = float4(0, 0, 0, 0);

				//Center
				//float sideValue = -1.0/9.0;
				//Bottom
				//sum += tex2D(tex, (uv + float2(-1.0, -1.0) * _MainTex_TexelSize)) *  0;
				sum += tex2D(tex, (uv + float2( 1.0, -1.0) * _MainTex_TexelSize)) * -1;
				//sum += tex2D(tex, (uv + float2( 1.0,  -1.0) * _MainTex_TexelSize)) *  0;

				//Center
				sum += tex2D(tex, (uv + float2( -1.0,  0.0) * _MainTex_TexelSize)) * -1;
				sum += tex2D(tex, (uv + float2( 0.0,  0.0) * _MainTex_TexelSize)) *  5;
				sum += tex2D(tex, (uv + float2( 1.0,  0.0) * _MainTex_TexelSize)) * -1;
				
				//Top
				//sum += tex2D(tex, (uv + float2(-1.0, 1.0) * _MainTex_TexelSize)) *  0;
				sum += tex2D(tex, (uv + float2( 0.0, 1.0) * _MainTex_TexelSize)) *  -1;
				//sum += tex2D(tex, (uv + float2( 1.0, 1.0) * _MainTex_TexelSize)) *  0;

				return float4(sum.xyz, 1.0);
			}
		
			float4 frag (v2f_img IN) : COLOR {
				float s = (Sharp(_MainTex, IN.uv)  >= _Threshold) ? 1.0 : 0.0;
				//float s = Sharp(_MainTex, IN.uv);
				return float4(s, s, s, 1);
			}
		ENDCG
		}

		//Blending
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;
			float _BlendTransition;
			uniform sampler2D _BlendTex;
		
			float4 frag (v2f_img IN) : COLOR {
				fixed4 presentTex = tex2D(_MainTex, IN.uv);
				fixed4 previousTex = tex2D(_BlendTex, IN.uv);

				fixed4 blendTex = lerp(previousTex, presentTex, _BlendTransition);

				return blendTex;
			}
		ENDCG
		}
		//Erosion
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;
			float _Threshold;

			float Dilation (sampler2D tex, float2 uv) {
			
				float4 sum = tex2D(tex, (uv));


				sum = min(sum, tex2D(tex, (uv + (float2(-1.0, -1.0) * _MainTex_TexelSize))));
				sum = min(sum,tex2D(tex, (uv + (float2( 0.0, -1.0) * _MainTex_TexelSize))));
				sum = min(sum,tex2D(tex, (uv + (float2( 1.0,  -1.0) * _MainTex_TexelSize))));

				//Center
				sum = min(sum,tex2D(tex, (uv + (float2( -1.0,  0.0) * _MainTex_TexelSize))));
				sum = min(sum,tex2D(tex, (uv + (float2( 1.0,  0.0) * _MainTex_TexelSize))));
				
				//Top
				sum = min(sum,tex2D(tex, (uv + (float2(-1.0, 1.0) * _MainTex_TexelSize))));
				sum = min(sum,tex2D(tex, (uv + (float2( 0.0, 1.0) * _MainTex_TexelSize))));
				sum = min(sum,tex2D(tex, (uv + (float2( 1.0, 1.0) * _MainTex_TexelSize))));

				return float4(sum.xyz , 1.0);
			}
		
			float4 frag (v2f_img IN) : COLOR {
				float s = Dilation(_MainTex, IN.uv);

				return float4(s, s, s, 1);
			}
		ENDCG
		}

		//Dilation
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
            #pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float2 _MainTex_TexelSize;
			float _Threshold;
			float _DeltaX;
			float _DeltaY;

			float Dilation (sampler2D tex, float2 uv) {
			
				float4 sum = tex2D(tex, (uv + float2( 0.0,  0.0) * _MainTex_TexelSize));

                 float2 _min = float2(0,0);
                 float2 _max = float2(1,1);
 
                 //get the color of 8 neighbour pixel
                 fixed4 U = tex2D(_MainTex,clamp(uv + float2(0,_DeltaY),_min,_max));
                 fixed4 UR = tex2D(_MainTex,clamp(uv + float2(_DeltaX,_DeltaY),_min,_max));
                 fixed4 R = tex2D(_MainTex,clamp(uv + float2(_DeltaX,0),_min,_max));
                 fixed4 DR = tex2D(_MainTex,clamp(uv + float2(_DeltaX,-_DeltaY),_min,_max));
                 fixed4 D = tex2D(_MainTex,clamp(uv + float2(0,-_DeltaY),_min,_max));
                 fixed4 DL = tex2D(_MainTex,clamp(uv + float2(-_DeltaX,-_DeltaY),_min,_max));
                 fixed4 L = tex2D(_MainTex,clamp(uv + float2(-_DeltaX,0),_min,_max));
                 fixed4 UL = tex2D(_MainTex,clamp(uv + float2(-_DeltaX,_DeltaY),_min,_max));
                 
                 //add all colors up to one final color
                 fixed4 finalColor = U + UR + R + DR + D + DL + L + UL;


				return float4(finalColor.xyz, 1.0);
			}
		
			float4 frag (v2f_img IN) : COLOR {
				float s = Dilation(_MainTex, IN.uv);

				return float4(s, s, s, 1);
			}
		ENDCG

		}


				Pass
			{
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#pragma target 3.0

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float2 _MainTex_TexelSize;

				float4 Dilation(sampler2D tex, float2 uv) {

					fixed4 center = tex2D(tex, (uv));

					//Center
					float sideValue = 1.0/8.0;
					//Bottom
					float4 sum = tex2D(tex, (uv + float2(-1.0, -1.0) * _MainTex_TexelSize)) *  1;
					sum += tex2D(tex, (uv + float2(0.0, -1.0) * _MainTex_TexelSize)) * 1;
					sum += tex2D(tex, (uv + float2( 1.0,  -1.0) * _MainTex_TexelSize)) *  1;

					//Center
					sum += tex2D(tex, (uv + float2(-1.0, 0.0) * _MainTex_TexelSize)) * 1;
					//sum += tex2D(tex, (uv + float2(0.0, 0.0) * _MainTex_TexelSize)) * 5;
					sum += tex2D(tex, (uv + float2(1.0, 0.0) * _MainTex_TexelSize)) * 1;

					//Top
					sum += tex2D(tex, (uv + float2(-1.0, 1.0) * _MainTex_TexelSize)) *  1;
					sum += tex2D(tex, (uv + float2(0.0, 1.0) * _MainTex_TexelSize)) * 1;
					sum += tex2D(tex, (uv + float2( 1.0, 1.0) * _MainTex_TexelSize)) *  1;

					sum *= sideValue;
					

					fixed4 finalValue = ((sum.x <= 0.15) ? 0 : 1) * (center.x);

					return float4(center.xyz, 1.0);
				}

				float4 frag(v2f_img IN) : COLOR {
					float4 s = Dilation(_MainTex, IN.uv);

					return s;
				}
			ENDCG

			}


    }
}
