/*
*
* CompositeShader.shader
* Use this file to create a awesome old tv effect.
*
* Version 3.0
*
* Developed by Vortex Game Studios LTDA ME. (http://www.vortexstudios.com)
* Authors:		Alexandre Ribeiro de Sa (@alexribeirodesa)
*				Luiz Fernando Ribeiro de Sa
*
*/

Shader "Vortex Game Studios/Filters/OLD TV Filter/Composite" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_LineCount("Composite Video Line count", float) = 200
		_Distortion("Composite Video Distortion", float) = 0.5
		_Gamma("Composite Video _Gamma", float) = 0.2
		_Artifact("Composite Video _Artifact", float) = 0.7
		_Fringing("Composite Video _Fringing", float) = 1.0
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

			half _LineCount;
			half _Distortion;
			half _Gamma;
			half _Artifact;
			half _Fringing;

			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _ScreenAspect;
			float _ScreenWidth;
			float _ScreenHeight;

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

			half Overlay(float I, float M) {
				I += 0.5;
				M += 0.5;
				return (I * (I + (2 * M)*(1 - I))) -0.5;
			}

			half4 frag(v2f i) : SV_Target{
				_ScreenAspect = _LineCount / _MainTex_TexelSize.w;
				_ScreenWidth = 1.0 / (_MainTex_TexelSize.z * _ScreenAspect);
				_ScreenHeight = 1.0 / (_MainTex_TexelSize.w * _ScreenAspect);

				int even = fmod(abs(i.uv.x / -_ScreenWidth * 0.5 + i.uv.y / _ScreenHeight) + 0.5, 2);
				_Distortion = _ScreenWidth * (even - 0.5) * _Distortion;
				half4 output = tex2D(_MainTex, i.uv);
				output += tex2D(_MainTex, i.uv + half2(_Distortion,0));

				half3 yuv = RGB2YUV(output.rgb * 0.5);

				//	video composto
				half3 tc = half3(0.0, 0.0, 0.0);

				int t = _Time.y * 20.0;
				t = fmod(t, 2);

				tc = tex2D(_MainTex, i.uv + half2(-_ScreenWidth+ _Distortion, 0)).rgb * 1.0;
				tc += tex2D(_MainTex, i.uv + half2(_Distortion, 0)).rgb * -2.0;
				tc += tex2D(_MainTex, i.uv + half2(_ScreenWidth+ _Distortion, 0)).rgb * 1.0;

				half c = RGB2Y(tc);
				c = clamp(c, 0.0, 1.0);
				if (even > 0)
					c = -c;

				if (t > 0)
					c = -c;
			

				_Artifact = c*_Artifact;
				_Fringing = c*_Fringing;
				
				yuv.r = Overlay(yuv.r, _Artifact);
				yuv.g = Overlay(yuv.g, _Fringing);
				yuv.b = Overlay(yuv.b, _Fringing);

				output.rgb = YUV2RGB(yuv);


				output.a = 1.0;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback off
}
