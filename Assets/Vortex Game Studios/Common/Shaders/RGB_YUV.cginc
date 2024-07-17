#ifndef __RGB_YUV__
#define __RGB_YUV__

//#include "UnityCG.cginc"

inline float RGB2Y(float3 rgb) {
	return dot(rgb, float3(0.29900, 0.58700, 0.11400));
}

inline float RGB2U(float3 rgb) {
	return dot(rgb, float3(-0.14713, -0.28886, 0.43600));
}

inline float RGB2V(float3 rgb) {
	return dot(rgb, float3(0.61500, -0.51499, -0.10001));
}

float3 RGB2YUV(float3 rgb) {
	float3 yuv;
	yuv.r = RGB2Y(rgb);
	yuv.g = RGB2U(rgb);
	yuv.b = RGB2V(rgb);

	return yuv;
}

inline float YUV2R(float3 yuv) {
	return dot(yuv, float3(1, 0.00000, 1.13983));
}

inline float YUV2G(float3 yuv) {
	return dot(yuv, float3(1.0, -0.39465, -0.58060));
}

inline float YUV2B(float3 yuv) {
	return dot(yuv, float3(1.0, 2.03211, 0.00000));
}

float3 YUV2RGB(float3 yuv) {
	float3 rgb;
	rgb.r = YUV2R(yuv);
	rgb.g = YUV2G(yuv);
	rgb.b = YUV2B(yuv);

	return rgb;
}

#endif // __RGB_YUV__
