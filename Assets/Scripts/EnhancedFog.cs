using UnityEngine;

public static class EnhancedFog {
    public static class ShaderPropertyID {
        public static readonly int g_EnhancedFogGradientTex = Shader.PropertyToID("g_EnhancedFogGradientTex");
        public static readonly int g_EnhancedFogColor = Shader.PropertyToID("g_EnhancedFogColor");
        public static readonly int g_EnhancedFogParams = Shader.PropertyToID("g_EnhancedFogParams");
        public static readonly int g_EnhancedFogColorMode = Shader.PropertyToID("g_EnhancedFogColorMode");
    }

    private static bool m_isEnabled = false;
    private static EnhancedFogColorMode m_colorMode = EnhancedFogColorMode.SingleColor;
    private static Color m_color = Color.gray;
    private static Gradient m_gradient = new Gradient();
    private static Texture2D m_gradientTexture;
    private static EnhancedFogMode m_mode = EnhancedFogMode.Linear;
    private static float m_density = 0.01f;
    private static float m_startDistance = 0.0f;
    private static float m_endDistance = 100.0f;

    public static void ApplyFogSettings(EnhancedFogSettings fogSettings) {
        if (fogSettings != null) {
            isEnabled = fogSettings.isEnabled;
            colorMode = fogSettings.colorMode;
            color = fogSettings.color;
            gradient = fogSettings.gradient;
            gradientTexture = fogSettings.gradientTexture;
            mode = fogSettings.mode;
            density = fogSettings.density;
            startDistance = fogSettings.startDistance;
            endDistance = fogSettings.endDistance;
        }
    }

    public static bool isEnabled {
        get { return m_isEnabled; }
        set {
            m_isEnabled = value;
            ApplyShaderKeywords();
        }
    }

    public static EnhancedFogColorMode colorMode {
        get { return m_colorMode; }
        set {
            m_colorMode = value;
            Shader.SetGlobalFloat(ShaderPropertyID.g_EnhancedFogColorMode, (float)value);
        }
    }

    public static Color color {
        get { return m_color; }
        set {
            m_color = value;
            Shader.SetGlobalColor(ShaderPropertyID.g_EnhancedFogColor, value);
        }
    }

    public static Gradient gradient {
        get { return m_gradient; }
        set { 
            m_gradient = value;
            gradientTexture = GenerateGradientTexture(value);
        }
    }

    public static Texture2D gradientTexture {
        get { return m_gradientTexture; }
        set {
            m_gradientTexture = value;
            Shader.SetGlobalTexture(ShaderPropertyID.g_EnhancedFogGradientTex, value);
        }
    }

    public static EnhancedFogMode mode {
        get { return m_mode; }
        set { m_mode = value; }
    }

    public static float density {
        get { return m_density; }
        set {
            m_density = value;
            ApplyGlobalEnhancedFogParamsChanges();
        }
    }

    public static float startDistance {
        get { return m_startDistance; }
        set {
            m_startDistance = value;
            ApplyGlobalEnhancedFogParamsChanges();
        }
    }

    public static float endDistance {
        get { return m_endDistance; }
        set {
            m_endDistance = value;
            ApplyGlobalEnhancedFogParamsChanges();
        }
    }

    private static void ApplyGlobalEnhancedFogParamsChanges() {
        Vector4 fogParams = new Vector4(
            density / Mathf.Sqrt(Mathf.Log(2)),
            density / Mathf.Log(2),
            -1 / (endDistance - startDistance),
            endDistance / (endDistance - startDistance)
        );

        Shader.SetGlobalVector(ShaderPropertyID.g_EnhancedFogParams, fogParams);
    }

    private static void ApplyShaderKeywords() {
        Shader.DisableKeyword("ENHANCEDFOG_LINEAR");
        Shader.DisableKeyword("ENHANCEDFOG_EXP");
        Shader.DisableKeyword("ENHANCEDFOG_EXP2");

        if (isEnabled) {
            switch (mode) {
                case EnhancedFogMode.Linear:
                    Shader.EnableKeyword("ENHANCEDFOG_LINEAR");
                    break;
                case EnhancedFogMode.Exponential:
                    Shader.EnableKeyword("ENHANCEDFOG_EXP");
                    break;
                case EnhancedFogMode.ExponentialSquared:
                    Shader.EnableKeyword("ENHANCEDFOG_EXP2");
                    break;
            }
        }
    }

    private static Texture2D GenerateGradientTexture(Gradient gradient, int textureWidth = 128) {
        Texture2D gradientTexture = new Texture2D(textureWidth, 1, TextureFormat.ARGB32, false );
        gradientTexture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < textureWidth; i++) {
            float progress = i / (textureWidth - 1.0f);
            gradientTexture.SetPixel(i, 0, gradient.Evaluate(progress));
        }

        gradientTexture.Apply();
        return gradientTexture;
    }
}
