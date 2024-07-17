/*
*
* ChromaticAberrationShader.shader
* Use this file to create a awesome old tv effect.
*
* Version 3.0
*
* Developed by Vortex Game Studios LTDA ME. (http://www.vortexstudios.com)
* Authors:		Alexandre Ribeiro de Sa (@alexribeirodesa)
*				Luiz Fernando Ribeiro de Sa
*
*/

Shader "Vortex Game Studios/Filters/OLD TV Filter/ChromaticAberration" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_MaskTex("Mask Pattern (RGB)", 2D) = "black" {}
	}

	SubShader {
		Pass {
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "../../../Common/Shaders/RGB_YUV.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _MaskTex;
			uniform half _Magnitude;

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
				half3 c = tex2D(_MaskTex, i.uv)*_Magnitude;

				half4 output;
				output.rgb = half3( tex2D(_MainTex, i.uv - c.r).r,
									tex2D(_MainTex, i.uv).g,
									tex2D(_MainTex, i.uv + c.b).b);

				output.a = 1.0;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback off
}
