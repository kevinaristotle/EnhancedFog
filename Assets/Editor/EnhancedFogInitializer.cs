﻿using System;
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
        EditorSceneManager.newSceneCreated += OnNewSceneCreated;
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
        fogSettings = GetOrCreateFogSettings(scene);
        RenderFogSettings();
        EditorApplication.update -= InitialUpdate;
    }

    private static void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode) {
        fogSettings = ScriptableObject.CreateInstance(typeof(EnhancedFogSettings)) as EnhancedFogSettings;
        EnhancedFog.currentFogSettings = fogSettings;
        fogSettings.Render();
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode) {
        Debug.Log("OnSceneOpened");
        Debug.Log("SceneName = " + scene.name);
        fogSettings = GetOrCreateFogSettings(scene);
        RenderFogSettings();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (mode == LoadSceneMode.Single) {
            EnhancedFog.currentFogSettings = GetOrCreateFogSettings(scene);
        }
    }

    private static void OnPlaymodeStateChanged(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredEditMode) {
            Scene activeScene = SceneManager.GetActiveScene();
            fogSettings = GetOrCreateFogSettings(activeScene);
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
                EnhancedFogSettings fogSettings = GetOrCreateFogSettings(scenePath);
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

    public static EnhancedFogSettings GetOrCreateFogSettings(string scenePath) {
        EnhancedFogSettings loadedFogSettings = EnhancedFogLoader.GetFogSettings(scenePath);
        if (!loadedFogSettings) {
            if (EnhancedFogSettingsProvider.autoGenerateFogSettings) {
                loadedFogSettings = EnhancedFogLoader.CreateFogSettings(scenePath);
            } else {
                loadedFogSettings = ScriptableObject.CreateInstance(typeof(EnhancedFogSettings)) as EnhancedFogSettings;
            }
        }

        return loadedFogSettings;
    }

    public static EnhancedFogSettings GetOrCreateFogSettings(Scene scene) {
       return GetOrCreateFogSettings(scene.path);
    }

    private static bool IsAssetFolderPathADirectory(string path) {
        string assets = "Assets";
        string applicationDataPath = Application.dataPath;
        string absoluteAssetDir = applicationDataPath.Remove(applicationDataPath.Length - assets.Length, assets.Length) + path;

        return Directory.Exists(absoluteAssetDir);
    }
}
