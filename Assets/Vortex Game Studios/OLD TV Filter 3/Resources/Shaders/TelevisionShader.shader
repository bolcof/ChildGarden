/*
*
* TelevisionShader.shader
* Use this file to create a awesome old tv effect.
*
* Version 3.0
*
* Developed by Vortex Game Studios LTDA ME. (http://www.vortexstudios.com)
* Authors:		Alexandre Ribeiro de Sa (@alexribeirodesa)
*				Luiz Fernando Ribeiro de Sa
*
*/

Shader "Vortex Game Studios/Filters/OLD TV Filter/Television" {
	Properties {
		_MainTex("Base (RGB)", 2D) = "white" {}
		//_Sync("Horizontal/Vertical Sync", float2) = 0.0;
		_Brightness("Screen Brightness", float) = 0.0
		_Contrast("Screen Contrast", float) = 0.0
		_Saturation("Screen Saturation", float) = 0.5
		_Sharpness("Screen Sharpness", float) = -1.0
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

			half _ScreenWidth;
			half _ScreenHeight;
			half2 _Sync;
			half _Brightness;
			half _Contrast;
			half _Saturation;
			half _Sharpness;
			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;

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
				return (I*(I + (2 * (M + 0.5))*(1 - I)));
			}

			half4 frag(v2f i) : SV_Target {
				half4 output;
				// I need to get a old tv with this to see how it works
				//_Sync.x *= 2.0*(i.uv.y-0.5);
				//output = tex2D(_MainTex, i.uv + _Sync);
				output = tex2D(_MainTex, i.uv);
				half3 yuv = RGB2YUV(output.rgb);
				
				half _Kernel[3]; _Kernel[0] = -1; _Kernel[1] = 2; _Kernel[2] = -1;

				if (_Sharpness <= 0) {
					_Kernel[0] = 1; _Kernel[1] = 3; _Kernel[2] = 1;				
				}

				if (_Sharpness != 0.0) {
					half3 yuvComposite = yuv;
					yuvComposite = 0;

					half x = 0;
					half px = 0;

					for (x = -1; x <= 1; x++) {
						px = x * _ScreenWidth;
						yuvComposite += RGB2YUV(tex2D(_MainTex, i.uv + half2(px, 0) + _Sync).rgb) * _Kernel[x + 1];
					}


					if (_Sharpness > 0) {
						yuv.r = Overlay(yuv.r, yuvComposite.r * _Sharpness);
					} else {
						yuvComposite /= 5;
						half localSharpness = (1 + _Sharpness);
						yuv = (yuv * localSharpness) + (yuvComposite * (1 - localSharpness) );
					}
				}

				/*
				//	Y = ((Y - 16) x C) + B + 16
				//	U - 0 ~ 255
				//	U = (U - 128) x Cos(H) + (V - 128) x Sin(H)
				//	V = (V - 128) x Cos(H) - (U - 128) x Sin(H)
				//	U = (((U - 128) x Cos(H) + (V - 128) x Sin(H)) x C x S) + 128
				//	V = (((V - 128) x Cos(H) - (U - 128) x Sin(H)) x C x S) + 128
				//	https://docs.microsoft.com/en-us/windows-hardware/drivers/display/processing-in-the-8-bit-yuv-color-space
				*/

				float cs = _Contrast * _Saturation;

				yuv.r = ((yuv.r - 0.062745) * _Contrast) + _Brightness + 0.062745;
				yuv.g *= cs;
				yuv.b *= cs;
				
				output.rgb = YUV2RGB(yuv);
				output.a = 1.0;
				return output;
			}
			ENDCG
		}
	}
	
	Fallback off
}
