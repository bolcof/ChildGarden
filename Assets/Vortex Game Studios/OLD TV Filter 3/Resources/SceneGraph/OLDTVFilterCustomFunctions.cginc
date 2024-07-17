#ifndef __OLDTVFILTER_CUSTOM_FUNCTIONS__
#define __OLDTVFILTER_CUSTOM_FUNCTIONS__

#include "../../../Common/Shaders/RGB_YUV.cginc"

// tube shader custom node
void TubeDistortion_float(float2 uv, float distortion, out float2 Out) {
	float2 cc = uv - 0.5;
	float dt = dot(cc, cc) * distortion;
	dt -= 0.2 * distortion;
	Out = uv + cc * (1.0 + dt) * dt;

	//if (i.uv.x < 0.0025 || i.uv.x > 0.9975 || i.uv.y < 0.0025 || i.uv.y > 0.9975) {
	//	return float4(0.0, 0.0, 0.0, 1.0);
	//}
}

void TubeReflex_float(float4 Texture, float4 ReflexTex, float ReflexMagnitude, out float4 Out) {
	float4 r = ReflexTex * ReflexMagnitude * (1.0 - RGB2Y(Texture.rgb));
	Out = Texture + r;
}

// scanline custom node
void ScanLine_float(float4 Texture, float4 Scanline, float Magnitude, out float4 Out) {
	Scanline *= (1.0 - Magnitude);
	Scanline += Magnitude;

	half over = RGB2Y(Texture.rgb);

	Scanline.rgb = RGB2YUV(Scanline.rgb);
	Scanline.r += (over * over * over) * (0.75 * (1.0 - Magnitude));
	if (Scanline.r > 1.0) Scanline.r = 1.0;
	Scanline.rgb = YUV2RGB(Scanline.rgb);

	Texture.rgb *= Scanline.rgb;
	Texture.a = 1.0;

	Out = Texture;
}

void BasicSum_float(float A, float B, out float3 Out) {
	Out = A + B;
}

#endif // __OLDTVFILTER_CUSTOM_FUNCTIONS__