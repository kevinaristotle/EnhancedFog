#ifndef ENHANCED_FOG_CGINC
#define ENHANCED_FOG_CGINC

uniform fixed4 g_EnhancedFogColor;
uniform fixed4 g_EnhancedFogParams;

inline float CalculateFogFactor(float coords) {
    #if defined(ENHANCEDFOG_LINEAR)
        return saturate(coords * g_EnhancedFogParams.z + g_EnhancedFogParams.w);
    #elif defined(ENHANCEDFOG_EXP)
        float fogFactor = g_EnhancedFogParams.y * coords;
        fogFactor = exp2(-fogFactor);
        return saturate(fogFactor);
    #elif defined(ENHANCEDFOG_EXP2)
        float fogFactor = g_EnhancedFogParams.x * coords;
        fogFactor = exp2(-fogFactor * fogFactor);
        return saturate(fogFactor);
    #else
        return 1;
    #endif
}

float3 ApplyFog(fixed3 color, float3 worldPos) {
	float viewDistance = length(_WorldSpaceCameraPos - worldPos);
    float fogFactor = CalculateFogFactor(viewDistance);
	return lerp(g_EnhancedFogColor.rgb, color, fogFactor);
}

#endif