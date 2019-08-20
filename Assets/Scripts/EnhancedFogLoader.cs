using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public static class EnhancedFogLoader {
#if UNITY_EDITOR
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

    private static EnhancedFogSettings CreateFogSettings(string assetDir) {
        string assets = "Assets";
        string applicationDataPath = Application.dataPath;
        int applicationDataPathStringLen = applicationDataPath.Length;
        string absoluteAssetDir = applicationDataPath.Remove(applicationDataPath.Length - assets.Length, assets.Length) + "/" + assetDir;
        Debug.Log("Checking for directory: " + absoluteAssetDir);
        if (!System.IO.Directory.Exists(absoluteAssetDir)) {
            System.IO.Directory.CreateDirectory(absoluteAssetDir);
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

    public static EnhancedFogSettings GetFogSettings(string scenePath)
    {
        if (string.IsNullOrEmpty(scenePath))
        {
            return null;
        }

        string sceneName = Path.GetFileNameWithoutExtension(scenePath);

        string fogSettingsPath = GetFogSettingsPath(sceneName, scenePath);
        Debug.Log("Fog Settings Path = " + fogSettingsPath);
        EnhancedFogSettings fogSettings = (EnhancedFogSettings)AssetDatabase.LoadAssetAtPath(fogSettingsPath, typeof(EnhancedFogSettings));
        return fogSettings;

    }

    public static EnhancedFogSettings GetFogSettings(Scene scene) {
        if (string.IsNullOrEmpty(scene.path)) {
            return null;
        }

        string fogSettingsPath = GetFogSettingsPath(scene.name, scene.path);
        EnhancedFogSettings fogSettings = (EnhancedFogSettings)AssetDatabase.LoadAssetAtPath(fogSettingsPath, typeof(EnhancedFogSettings));
        return fogSettings;
    }

    public static EnhancedFogSettings CreateFogSettings(Scene scene) {
        if (string.IsNullOrEmpty(scene.path)) {
            return null;
        }

        string fogSettingsPath = GetFogSettingsPath(scene.name, scene.path);
        EnhancedFogSettings fogSettings = (EnhancedFogSettings)AssetDatabase.LoadAssetAtPath(fogSettingsPath, typeof(EnhancedFogSettings));

        if (!fogSettings) {
            string fogSettingsDir = GetFogSettingsDir(scene.name, scene.path);
            fogSettings = CreateFogSettings(fogSettingsDir);
        }

        return fogSettings;
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
