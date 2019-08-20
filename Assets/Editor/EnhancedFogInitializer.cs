using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class EnhancedFogInitializer {
    private static EnhancedFogSettings fogSettings;

    static EnhancedFogInitializer() {
        Debug.Log("EnhancedFogInitializer");
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorApplication.update += InitialUpdate;
        EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
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

    private static void OnPlaymodeStateChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) {
            Scene activeScene = SceneManager.GetActiveScene();
            fogSettings = GetFogSettings(activeScene);
            RenderFogSettings();
        }
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
}
