using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnhancedFogSettingsProvider : SettingsProvider
{
    private class EnhancedFogProjectSettings : ScriptableObject
    {
        [SerializeField]
        internal bool autoGenerateFogSettings;

        internal static EnhancedFogProjectSettings GetOrCreateSettings() {
            var settings = AssetDatabase.LoadAssetAtPath<EnhancedFogProjectSettings>(EnhancedFogProjectSettingsPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<EnhancedFogProjectSettings>();
                settings.autoGenerateFogSettings = false;
                AssetDatabase.CreateAsset(settings, EnhancedFogProjectSettingsPath);
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        internal static SerializedObject GetSerializedSettings() {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    private static readonly GUIContent autoGenerateFogSettingsText = new GUIContent("Auto Generate Fog Settings");
    private static readonly string EnhancedFogProjectSettingsPath = "Assets/Editor/EnhancedFogProjectSettings.asset";

    private SerializedObject enhancedFogProjectSettings;
    private SerializedProperty autoGenerateFogSettingsProperty;

    public EnhancedFogSettingsProvider(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) {}

    public override void OnActivate(string searchContext, VisualElement rootElement) {
        enhancedFogProjectSettings = EnhancedFogProjectSettings.GetSerializedSettings();
        autoGenerateFogSettingsProperty = enhancedFogProjectSettings.FindProperty("autoGenerateFogSettings");
    }

    public override void OnGUI(string searchContext) {
        EditorGUI.indentLevel = 1;
        autoGenerateFogSettingsProperty.boolValue = EditorGUILayout.ToggleLeft(autoGenerateFogSettingsText, autoGenerateFogSettingsProperty.boolValue);
        enhancedFogProjectSettings.ApplyModifiedProperties();
    }

    [SettingsProvider]
    private static SettingsProvider CreateEnhancedFogSettingsProvider() {
        var provider = new EnhancedFogSettingsProvider("Project/Enhanced Fog", SettingsScope.Project);
        provider.keywords = GetSearchKeywordsFromGUIContentProperties<EnhancedFogSettingsProvider>();
        return provider;
    }

    public bool autoGenerateFogSettings {
        get { 
            if (enhancedFogProjectSettings != null) {
                return enhancedFogProjectSettings.FindProperty("autoGenerateFogSettings").boolValue;
            }

            return false;
        }
    }
}
