#ifndef ENHANCED_FOG_CGINC
#define ENHANCED_FOG_CGINC

uniform sampler2D g_EnhancedFogGradientTex;
uniform fixed4 g_EnhancedFogColor;
uniform fixed4 g_EnhancedFogParams;
uniform fixed g_EnhancedFogColorMode;

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
    float3 gradientColor = tex2D(g_EnhancedFogGradientTex, float2(1 - fogFactor, 0)).rgb;
    float3 fogColor = lerp(g_EnhancedFogColor.rgb, gradientColor, g_EnhancedFogColorMode);
	return lerp(fogColor, color, fogFactor);
}

#endif