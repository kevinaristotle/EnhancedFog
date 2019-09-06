using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EnhancedFogInitializer {
    private static EnhancedFogSettings fogSettings;

    private static bool attemptingToCreateFogSettings;
    private static string recentlyCreatedAssetPath;

    static EnhancedFogInitializer() {
        Debug.Log("EnhancedFogInitializer");
        EditorSceneManager.sceneOpened += OnSceneOpened;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EditorApplication.update += InitialUpdate;
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        EditorApplication.projectChanged += OnProjectChanged;
    }

    public static void CreateFogSettingsForCreatedAsset(string path) {
        attemptingToCreateFogSettings = true;
        recentlyCreatedAssetPath = path;
    }

    private static void InitialUpdate() {
        Scene scene = SceneManager.GetActiveScene();
        fogSettings = GetFogSettings(scene);
        RenderFogSettings();
        EditorApplication.update -= InitialUpdate;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode) {
        Debug.Log("OnSceneOpened");
        Debug.Log("SceneName = " + scene.name);
        fogSettings = GetFogSettings(scene);
        RenderFogSettings();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Single) {
            EnhancedFog.currentFogSettings = EnhancedFogLoader.GetFogSettings(scene);
        }
    }

    private static void OnPlaymodeStateChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) {
            Scene activeScene = SceneManager.GetActiveScene();
            fogSettings = GetFogSettings(activeScene);
            RenderFogSettings();
        }
    }

    private static void OnProjectChanged() {
        if (attemptingToCreateFogSettings) {
            string path = recentlyCreatedAssetPath;
            string sceneDir = Path.GetDirectoryName(path);
            string sceneName = Path.GetFileNameWithoutExtension(path);
            string scenePath = Path.Combine(sceneDir, sceneName);

            if (!IsAssetFolderPathADirectory(scenePath)) {
                EnhancedFogSettings fogSettings = EnhancedFogLoader.GetFogSettings(scenePath);
                if (!fogSettings) {
                    fogSettings = EnhancedFogLoader.CreateFogSettings(scenePath);
                }
            }
        }

        attemptingToCreateFogSettings = false;
        recentlyCreatedAssetPath = String.Empty;

    }

    private static void RenderFogSettings() {
        if (fogSettings) {
            fogSettings.Render();
        }
    }

    private static EnhancedFogSettings GetFogSettings(Scene scene) {
        EnhancedFogSettings loadedFogSettings = EnhancedFogLoader.GetFogSettings(scene);
        if (!loadedFogSettings) {
            loadedFogSettings = EnhancedFogLoader.CreateFogSettings(scene);
        }

        return loadedFogSettings;
    }

    private static bool IsAssetFolderPathADirectory(string path) {
        string assets = "Assets";
        string applicationDataPath = Application.dataPath;
        int applicationDataPathStringLen = applicationDataPath.Length;
        string absoluteAssetDir = applicationDataPath.Remove(applicationDataPath.Length - assets.Length, assets.Length) + path;

        return Directory.Exists(absoluteAssetDir);
    }
}
