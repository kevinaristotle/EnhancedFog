using System.IO;

public class EnhancedFogAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    static void OnWillCreateAsset(string path)
    {
        if (path.Contains(".unity.meta"))
        {
            string sceneDir = Path.GetDirectoryName(path);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            string scenePath = Path.Combine(sceneDir, sceneName);

            EnhancedFogSettings fogSettings = EnhancedFogLoader.GetFogSettings(scenePath);
            if (!fogSettings) {
                fogSettings = EnhancedFogLoader.CreateFogSettings(scenePath);
            }
        }
    }
}
