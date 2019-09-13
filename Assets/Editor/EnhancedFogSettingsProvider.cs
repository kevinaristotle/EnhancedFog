using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnhancedFogSettingsProvider : SettingsProvider
{
    private class EnhancedFogProjectSettings : ScriptableObject
    {
        [SerializeField]
        private bool m_autoGenerateFogSettings;

        internal static EnhancedFogProjectSettings GetOrCreateSettings() {
            var settings = AssetDatabase.LoadAssetAtPath<EnhancedFogProjectSettings>(k_enhancedFogProjectSettingsPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<EnhancedFogProjectSettings>();
                settings.m_autoGenerateFogSettings = false;
                AssetDatabase.CreateAsset(settings, k_enhancedFogProjectSettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings() {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    private static readonly GUIContent k_autoGenerateFogSettingsText = new GUIContent("Auto Generate Fog Settings");
    private static readonly string k_enhancedFogProjectSettingsPath = "Assets/Editor/EnhancedFogProjectSettings.asset";

    private SerializedObject m_enhancedFogProjectSettings;
    private SerializedProperty m_autoGenerateFogSettingsProperty;

    public EnhancedFogSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) {}

    public override void OnActivate(string searchContext, VisualElement rootElement) {
        m_enhancedFogProjectSettings = EnhancedFogProjectSettings.GetSerializedSettings();
        m_autoGenerateFogSettingsProperty = m_enhancedFogProjectSettings.FindProperty("m_autoGenerateFogSettings");
    }

    public override void OnGUI(string searchContext) {
        EditorGUI.indentLevel = 1;
        m_autoGenerateFogSettingsProperty.boolValue = EditorGUILayout.ToggleLeft(k_autoGenerateFogSettingsText, m_autoGenerateFogSettingsProperty.boolValue);
        m_enhancedFogProjectSettings.ApplyModifiedProperties();
    }

    [SettingsProvider]
    private static SettingsProvider CreateEnhancedFogSettingsProvider() {
        var provider = new EnhancedFogSettingsProvider("Project/Enhanced Fog", SettingsScope.Project);
        provider.keywords = GetSearchKeywordsFromGUIContentProperties<EnhancedFogSettingsProvider>();
        return provider;
    }

    public static bool autoGenerateFogSettings {
        get {
            SerializedObject enhancedFogProjectSettings = EnhancedFogProjectSettings.GetSerializedSettings();
            if (enhancedFogProjectSettings != null) {
                return enhancedFogProjectSettings.FindProperty("m_autoGenerateFogSettings").boolValue;
            }

            return false;
        }
    }
}
