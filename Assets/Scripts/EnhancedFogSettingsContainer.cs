using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnhancedFogSettingsContainer : MonoBehaviour
{
    public List<EnhancedFogSettings> fogSettings = new List<EnhancedFogSettings>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
        EnhancedFog.currentFogSettings = fogSettings[0];
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            EnhancedFog.currentFogSettings = fogSettings[scene.buildIndex];
        }
    }
}
