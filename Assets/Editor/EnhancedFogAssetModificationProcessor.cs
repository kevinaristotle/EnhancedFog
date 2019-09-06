using System.IO;

public class EnhancedFogAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    static void OnWillCreateAsset(string path)
    {
        if (path.Contains(".unity.meta"))
        {
            EnhancedFogInitializer.CreateFogSettingsForCreatedAsset(path);
        }
    }
}
