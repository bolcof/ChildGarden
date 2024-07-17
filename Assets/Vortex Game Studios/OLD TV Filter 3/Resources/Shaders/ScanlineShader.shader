/*
*
* ScablineShader.shader
* Use this file to create a awesome old tv effect.
*
* Version 3.0
*
* Developed by Vortex Game Studios LTDA ME. (http://www.vortexstudios.com)
* Authors:		Alexandre Ribeiro de Sa (@alexribeirodesa)
*
*/

Shader "Vortex Game Studios/Filters/OLD TV Filter/Scanline" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_PatternTex("Scanline Pattern (RGB)", 2D) = "white" {}
		_Magnitude("Scanline Magnitude", float) = 0.75
		_LineCount("Scanline Line count", float) = 200
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
			uniform sampler2D _PatternTex;
			
			int _ScreenWidth;
			int _ScreenHeight;
			half _Magnitude;
			half _LineCount;

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

			fixed4 frag(v2f i) : SV_Target {
				fixed3 scanLine = 1.0;
				fixed4 output;
				output = tex2D(_MainTex, i.uv);

				_ScreenWidth = _MainTex_TexelSize.z * (_LineCount/_MainTex_TexelSize.w);
				_ScreenHeight = _LineCount;
				
				/*
				output.r = clamp(output.r, 0, 0.95);
				output.g = clamp(output.g, 0, 0.95);
				output.b = clamp(output.b, 0, 0.95);
				*/

				if (_ScreenHeight > 0 ) {
					scanLine = tex2D(_PatternTex, i.uv * fixed2(_ScreenWidth, _ScreenHeight)).rgb;
					
					scanLine *= (1.0 - _Magnitude);
					scanLine += _Magnitude;
					
					half over = RGB2Y(output.rgb);

					scanLine = RGB2YUV(scanLine);
					scanLine.r += (over*over*over)*(0.75*(1-_Magnitude));
					if(scanLine.r > 1) scanLine.r = 1;
					scanLine = YUV2RGB(scanLine);

					output.rgb *= scanLine;
					
				}

				output.a = 1.0;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback off
}
