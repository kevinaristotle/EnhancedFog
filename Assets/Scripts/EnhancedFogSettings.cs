using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Determine if ApplyShaderKeywords() function should remain in this file.
//Perhaps it should be used in EnhancedFogSettingsWindow.cs instead.

//TODO: To simplify EhancedFog system, perhaps this source file should not handle
//writing to global shader properties. That should all be handled by EnhancedFog.cs.

public class EnhancedFogSettings : ScriptableObject {
    [SerializeField] private bool m_isEnabled = false;
    [SerializeField] private Color m_color = Color.gray;
    [SerializeField] private EnhancedFogMode m_mode = EnhancedFogMode.Linear;
    [SerializeField] private float m_density = 0.01f;
    [SerializeField] private float m_startDistance = 0.0f;
    [SerializeField] private float m_endDistance = 100.0f;

    public bool isEnabled {
        get { return m_isEnabled; }
        set {
            m_isEnabled = value;
            ApplyShaderKeywords();
        }
    }

    public Color color {
        get { return m_color; }
        set { m_color = value; }
    }

    public EnhancedFogMode mode {
        get { return m_mode; }
        set { 
            m_mode = value;
            ApplyShaderKeywords();
        }
    }

    public float density {
        get { return m_density; }
        set { m_density = value; }
    }

    public float startDistance {
        get { return m_startDistance; }
        set { m_startDistance = value; }
    }

    public float endDistance {
        get { return m_endDistance; }
        set { m_endDistance = value; }
    }

    public void Render() {
        ApplyShaderKeywords();

        Shader.SetGlobalColor(EnhancedFog.ShaderPropertyID.g_EnhancedFogColor, color);

        Vector4 fogParams = new Vector4(
            density / Mathf.Sqrt(Mathf.Log(2)),
            density / Mathf.Log(2),
            -1 / (endDistance - startDistance),
            endDistance / (endDistance - startDistance)
        );

        Shader.SetGlobalVector(EnhancedFog.ShaderPropertyID.g_EnhancedFogParams, fogParams);
    }

    private void ApplyShaderKeywords() {
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
}
