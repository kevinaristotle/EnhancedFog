using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

//TODO: Created EnhancedFog namespace. Put EhnacedFogMode and EnhancedFog within the EnhancedFog namespace.

public enum EnhancedFogMode {
    Linear,
    Exponential,
    ExponentialSquared
}

//TODO: Loading EnhancedFogSettings currently uses Unity Editor asset path. This will not work with
//standalone builds. Need to rewrite so that m_currentFogSettings is initialized properly for standalone builds.

public static class EnhancedFog {
    public static class ShaderPropertyID {
        public static readonly int g_EnhancedFogColor = Shader.PropertyToID("g_EnhancedFogColor");
        public static readonly int g_EnhancedFogParams = Shader.PropertyToID("g_EnhancedFogParams");
    }

    private static EnhancedFogSettingsContainer m_fogSettingsContainer;
    private static EnhancedFogSettings m_currentFogSettings;

    private static bool m_isEnabled = false;
    private static Color m_color = Color.gray;
    private static EnhancedFogMode m_mode = EnhancedFogMode.Linear;
    private static float m_density = 0.01f;
    private static float m_startDistance = 0.0f;
    private static float m_endDistance = 100.0f;

    //TODO: Move code in #if UNITY_EDITOR somewhere else
#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnRuntimeMethodLoad() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (!m_currentFogSettings) {
            currentFogSettings = EnhancedFogLoader.GetFogSettings(scene);
        }

        if (mode == LoadSceneMode.Single) {
            currentFogSettings = EnhancedFogLoader.GetFogSettings(scene);
        }
    }
#endif

    public static EnhancedFogSettings currentFogSettings {
        set {
            m_currentFogSettings = value;

            if (m_currentFogSettings != null) {
                Debug.Log("EnhancedFog: loaded currentFogSettings");
                isEnabled = m_currentFogSettings.isEnabled;
                color = m_currentFogSettings.color;
                mode = m_currentFogSettings.mode;
                density = m_currentFogSettings.density;
                startDistance = m_currentFogSettings.startDistance;
                endDistance = m_currentFogSettings.endDistance;
            }
        }
    }

    public static bool isEnabled {
        get { return m_isEnabled; }
        set {
            m_isEnabled = value;
            ApplyShaderKeywords();
        }
    }

    public static Color color {
        get { return m_color; }
        set {
            m_color = value;
            Shader.SetGlobalColor(ShaderPropertyID.g_EnhancedFogColor, value);
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
}
