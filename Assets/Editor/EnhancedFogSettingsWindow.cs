﻿using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

//TODO: When a new scene is opened EnhancedFogSettingsWindow doesn't show anything because
//EnhancedFogSettings doesn't exist for that scene. EnhancedFogSettings are only created when
//a scene is saved. So to handle this case, temporary settings should be created, and those
//temporary settings should be saved to disk when the new scene is saved for the first time.

public class EnhancedFogSettingsWindow : EditorWindow {
    private static readonly string blankspace = " ";
    private static readonly string fogSettingsText = "Fog Settings";
    private static readonly string windowTitle = "Enhanced Fog";

    private static readonly GUIContent isEnabledText = new GUIContent("Fog Enabled", "");
    private static readonly GUIContent colorText = new GUIContent("Color", "");
    private static readonly GUIContent modeText = new GUIContent("Mode", "");
    private static readonly GUIContent densityText = new GUIContent("Density", "");
    private static readonly GUIContent startDistanceText = new GUIContent("Start Distance", "");
    private static readonly GUIContent endDistanceText = new GUIContent("End Distance", "");

    private bool initializedWindow;
    private SerializedObject activeEnhancedFogSettingsSerializedObject;
    private EnhancedFogSettings activeEnhancedFogSettings;
    private Texture unityLogo;
    private GUIContent headerContent;

    private bool isEnabled;
    private Color color;
    private EnhancedFogMode mode;
    private float startDistance;
    private float endDistance;
    private float density;

    [MenuItem("EnhancedFog/Enhanced Fog Settings")]
    public static void ShowWindow() {
        GetWindow(typeof(EnhancedFogSettingsWindow), false, windowTitle);
    }

    private void Awake() {
        InitializeWindow();
    }

    private void OnDestroy() {
        if (initializedWindow) {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.newSceneCreated -= OnNewSceneCreatedCallback;
            Undo.undoRedoPerformed -= OnUndoRedo;
        }
    }

    void OnInspectorUpdate() {
        if (!initializedWindow) {
            InitializeWindow();
        }

        Repaint();
    }

    private void OnGUI() {
        if (!activeEnhancedFogSettings) {
            return;
        }

        EditorGUILayout.LabelField(headerContent, EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUI.BeginChangeCheck(); {
            isEnabled = EditorGUILayout.Toggle(isEnabledText, activeEnhancedFogSettings.isEnabled);
            color = EditorGUILayout.ColorField(colorText, activeEnhancedFogSettings.color);
            mode = (EnhancedFogMode)EditorGUILayout.EnumPopup(modeText, activeEnhancedFogSettings.mode);
            if (activeEnhancedFogSettings.mode == EnhancedFogMode.Linear) {
                startDistance = EditorGUILayout.FloatField(startDistanceText, activeEnhancedFogSettings.startDistance);
                endDistance = EditorGUILayout.FloatField(endDistanceText, activeEnhancedFogSettings.endDistance);
            } else {
                density = EditorGUILayout.FloatField(densityText, activeEnhancedFogSettings.density);
            }
        } if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(activeEnhancedFogSettings, "Changed Enhanced Fog Settings");
            activeEnhancedFogSettings.isEnabled = isEnabled;
            activeEnhancedFogSettings.color = color;
            activeEnhancedFogSettings.mode = mode;
            activeEnhancedFogSettings.startDistance = startDistance;
            activeEnhancedFogSettings.endDistance = endDistance;
            activeEnhancedFogSettings.density = density;
            activeEnhancedFogSettings.Render();
            SceneView.RepaintAll();
            EditorUtility.SetDirty(activeEnhancedFogSettings);
            AssetDatabase.SaveAssets();
        }
    }

    private void OnSceneOpened(Scene scene, OpenSceneMode mode) {
        Debug.Log("Scene Opened!");
        Initialize(scene);
    }

    private void OnActiveSceneChanged(Scene current, Scene next) {
        Debug.Log("Active Scene Changed!");
        Initialize(next);
    }

    private void OnNewSceneCreatedCallback(Scene scene, NewSceneSetup setup, NewSceneMode mode) {
        Debug.Log("New Scene Created!");
        Initialize(scene);
    }

    private void InitializeWindow() {
        Scene activeScene = SceneManager.GetActiveScene();
        Initialize(activeScene);

        SceneManager.activeSceneChanged += OnActiveSceneChanged;
        EditorSceneManager.sceneOpened += OnSceneOpened;
        EditorSceneManager.newSceneCreated += OnNewSceneCreatedCallback;
        Undo.undoRedoPerformed += OnUndoRedo;

        initializedWindow = true;
    }

    private void Initialize(Scene scene) {
        activeEnhancedFogSettings = EnhancedFogLoader.GetFogSettings(scene);
        if (!activeEnhancedFogSettings) {
            return;
        }

        unityLogo = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
        string headerText = string.Join(blankspace, blankspace + scene.name, fogSettingsText);
        headerContent = new GUIContent(headerText, unityLogo, "");
    }

    private void OnUndoRedo() {
        if (activeEnhancedFogSettings) {
            activeEnhancedFogSettings.Render();
            SceneView.RepaintAll();
        }
    }
}
