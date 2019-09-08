using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class EnhancedFogLoader {
#if UNITY_EDITOR
    private static string GetAbsolutePath(string path) {
        string assets = "Assets";
        string applicationDataPath = Application.dataPath;
        return applicationDataPath.Remove(applicationDataPath.Length - assets.Length, assets.Length) + path;
    }

    private static string GetFogSettingsDir(string sceneName, string scenePath) {
        string sceneDirectory = scenePath.Replace(sceneName + ".unity", "");
        string fogSettingsDir = sceneDirectory + sceneName;
        return fogSettingsDir;
    }

    private static string GetFogSettingsPath(string sceneName, string scenePath) {
        string sceneDirectory = scenePath.Replace(sceneName + ".unity", "");
        string fogSettingsPath = sceneDirectory + sceneName + "/fogSettings.asset";
        Debug.Log("Fog Settings Path = " + fogSettingsPath);
        return fogSettingsPath;
    }

    private static EnhancedFogSettings CreateFogSettingsInDirectory(string assetDir) {
        string absoluteAssetDir = GetAbsolutePath(assetDir);
        Debug.Log("Checking for directory: " + absoluteAssetDir);
        if (!Directory.Exists(absoluteAssetDir)) {
            Directory.CreateDirectory(absoluteAssetDir);
            AssetDatabase.Refresh();
            Debug.Log(absoluteAssetDir + " does not exist!");
        }

        string assetPath = assetDir + "/fogSettings.asset";

        Debug.Log("Creating Enhanced Fog Settings @ " + assetPath);
        EnhancedFogSettings fogSettingsAsset = ScriptableObject.CreateInstance<EnhancedFogSettings>();
        AssetDatabase.CreateAsset(fogSettingsAsset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return fogSettingsAsset;
    }

    public static EnhancedFogSettings GetFogSettings(string scenePath) {
        if (!File.Exists(GetAbsolutePath(scenePath)) || string.IsNullOrEmpty(scenePath)) {
            return null;
        }

        string sceneName = Path.GetFileNameWithoutExtension(scenePath);
        string fogSettingsPath = GetFogSettingsPath(sceneName, scenePath);
        EnhancedFogSettings fogSettings = (EnhancedFogSettings)AssetDatabase.LoadAssetAtPath(fogSettingsPath, typeof(EnhancedFogSettings));
        return fogSettings;

    }

    public static EnhancedFogSettings GetFogSettings(Scene scene) {
        return GetFogSettings(scene.path);
    }

    public static EnhancedFogSettings CreateFogSettings(string scenePath) {
        if (!File.Exists(GetAbsolutePath(scenePath)) || string.IsNullOrEmpty(scenePath)) {
            return null;
        }

        string sceneName = Path.GetFileNameWithoutExtension(scenePath);

        string fogSettingsPath = GetFogSettingsPath(sceneName, scenePath);
        EnhancedFogSettings fogSettings = (EnhancedFogSettings)AssetDatabase.LoadAssetAtPath(fogSettingsPath, typeof(EnhancedFogSettings));

        if (!fogSettings) {
            string fogSettingsDir = GetFogSettingsDir(sceneName, scenePath);
            fogSettings = CreateFogSettingsInDirectory(fogSettingsDir);
        }

        return fogSettings;
    }

    public static EnhancedFogSettings CreateFogSettings(Scene scene) {
        return CreateFogSettings(scene.path);
    }
#else
    private static EnhancedFogSettingsContainer m_fogSettingsContainer;

    public static EnhancedFogSettings GetFogSettings(Scene scene)
    {
        if (m_fogSettingsContainer == null)
        {
            if (scene.buildIndex == 0)
            {
                m_fogSettingsContainer = GameObject.Find("EnhancedFogSettingsContainer").GetComponent<EnhancedFogSettingsContainer>();
            }
        }

        return m_fogSettingsContainer.fogSettings[scene.buildIndex];
    }
#endif
}
