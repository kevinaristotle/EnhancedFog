using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.SceneManagement;

public class EnhancedFogScenePostProcessor : MonoBehaviour
{
    [PostProcessScene]
    public static void OnPostprocessScene()
    {
        Scene scene = SceneManager.GetActiveScene();

        if (scene.buildIndex == 0)
        {
            GameObject go = new GameObject();
            go.name = "EnhancedFogSettingsContainer";
            EnhancedFogSettingsContainer fogSettingsContainer = go.AddComponent<EnhancedFogSettingsContainer>();

            EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

            int buildScenesCount = buildScenes.Length;
            for (int i = 0; i < buildScenesCount; i++)
            {
                string scenePath = buildScenes[i].path;
                Debug.Log("OnPostProcessScene: Getting fog settings at scene path = " + scenePath);
                fogSettingsContainer.fogSettings.Add(EnhancedFogLoader.GetFogSettings(scenePath));
            }
        }
    }
}
