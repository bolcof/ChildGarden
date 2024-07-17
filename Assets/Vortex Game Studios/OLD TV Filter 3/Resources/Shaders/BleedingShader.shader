/*
*
* BleedingShader.shader
* Use this file to create a awesome old tv effect.
*
* Version 3.0
*
* Developed by Vortex Game Studios LTDA ME. (http://www.vortexstudios.com)
* Authors:		Alexandre Ribeiro de Sa (@alexribeirodesa)
*				Luiz Fernando Ribeiro de Sa
*
*/

Shader "Vortex Game Studios/Filters/OLD TV Filter/Bleeding" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LineCount("Composite Video Line count", float) = 200
		_Magnitude("Bleeding Magnitude", float) = 1.0
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM

			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "../../../Common/Shaders/RGB_YUV.cginc"

			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _ScreenAspect;
			float _ScreenWidth;
			float _ScreenHeight;
			half _LineCount;
			float _Magnitude;

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
			};

			v2f vert(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			half4 frag(v2f i) : SV_Target {
				_ScreenAspect = _LineCount / _MainTex_TexelSize.w;
				_ScreenWidth = 1.0 / (_MainTex_TexelSize.z * _ScreenAspect);
				_ScreenHeight = 1.0 / (_MainTex_TexelSize.w * _ScreenAspect);

				//	original matrix:
				//	{ 2.0f / 13.0f, 4.0f / 13.0f, 1.0f / 13.0f, 4.0f / 13.0f, 2.0f / 13.0f };
				float _Kernel[3];
				_Kernel[0] = 0.1538;
				_Kernel[1] = 0.3076;
				_Kernel[2] = 0.0769;
				//_Kernel[3] = 0.3076;
				//_Kernel[4] = 0.1538;

				half4 output = tex2D(_MainTex, i.uv);
				half3 yuv = half3(0, 0, 0);
				half3 rgb = half3(0, 0, 0);
				half hWidth = _ScreenWidth / 2;

				rgb = tex2D(_MainTex, i.uv + half2(-2 * _ScreenWidth - hWidth, 0)).rgb * _Kernel[0];
				rgb += tex2D(_MainTex, i.uv + half2(-_ScreenWidth - hWidth, 0)).rgb * _Kernel[1];
				rgb += tex2D(_MainTex, i.uv + half2(-hWidth, 0)).rgb * _Kernel[2];
				rgb += tex2D(_MainTex, i.uv + half2(_ScreenWidth - hWidth, 0)).rgb * _Kernel[1];
				rgb += tex2D(_MainTex, i.uv + half2(2 * _ScreenWidth - hWidth, 0)).rgb * _Kernel[0];

				yuv.r = RGB2Y(output.rgb);
				yuv.g = RGB2U(rgb);
				yuv.b = RGB2V(rgb);

				output.rgb = (output.rgb*(1.0-_Magnitude))+(YUV2RGB(yuv)*_Magnitude);
				output.a = 1.0;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback off
}
