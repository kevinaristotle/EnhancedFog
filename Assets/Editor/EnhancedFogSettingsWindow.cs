using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class EnhancedFogSettingsWindow : EditorWindow {
    private static readonly string blankspace = " ";
    private static readonly string fogSettingsText = "Fog Settings";
    private static readonly string windowTitle = "Enhanced Fog";

    private static readonly string helpBoxText = "A Fog Settings asset does not exist on disk for this scene. Would you like to create one?";

    private static readonly GUIContent isEnabledText = new GUIContent("Fog Enabled", "");
    private static readonly GUIContent colorModeText = new GUIContent("Color Mode", "");
    private static readonly GUIContent colorText = new GUIContent("Color", "");
    private static readonly GUIContent gradientText = new GUIContent("Gradient", "");
    private static readonly GUIContent gradientTextureText = new GUIContent("Gradient Texture", "");
    private static readonly GUIContent modeText = new GUIContent("Mode", "");
    private static readonly GUIContent densityText = new GUIContent("Density", "");
    private static readonly GUIContent startDistanceText = new GUIContent("Start Distance", "");
    private static readonly GUIContent endDistanceText = new GUIContent("End Distance", "");

    private static readonly int gradientTextureWidth = 128;

    private bool initializedWindow;
    private SerializedObject fogSettingsSerializedObject;
    private bool fogSettingsNotFoundOnDisk;
    private EnhancedFogSettings fogSettings;
    private Scene currentScene;
    private Texture unityLogo;
    private GUIContent headerContent;

    private bool isEnabled;
    private EnhancedFogColorMode colorMode;
    private Color color;
    private Gradient gradient;
    private Texture2D gradientTexture;
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
        if (!fogSettings) {
            return;
        }

        if (fogSettingsNotFoundOnDisk && !String.IsNullOrEmpty(currentScene.path)) {
            DrawCreateFogSettingsAssetButton();
        }

        EditorGUILayout.LabelField(headerContent, EditorStyles.boldLabel, GUILayout.Height(20));

        EditorGUI.BeginChangeCheck(); {
            isEnabled = EditorGUILayout.Toggle(isEnabledText, fogSettings.isEnabled);
            EditorGUI.BeginChangeCheck(); {
                colorMode = (EnhancedFogColorMode)EditorGUILayout.EnumPopup(colorModeText, fogSettings.colorMode);
            } if (EditorGUI.EndChangeCheck()) {
                gradientTexture = GenerateGradientTexture(gradient, gradientTextureWidth);
            }
            if (fogSettings.colorMode == EnhancedFogColorMode.SingleColor) {
                color = EditorGUILayout.ColorField(colorText, fogSettings.color);
            } else {
                EditorGUI.BeginChangeCheck(); {
                    gradient = EditorGUILayout.GradientField(gradientText, fogSettings.gradient);
                } if (EditorGUI.EndChangeCheck()) {
                    gradientTexture = GenerateGradientTexture(gradient, gradientTextureWidth);
                }
            }
            mode = (EnhancedFogMode)EditorGUILayout.EnumPopup(modeText, fogSettings.mode);
            if (fogSettings.mode == EnhancedFogMode.Linear) {
                startDistance = EditorGUILayout.FloatField(startDistanceText, fogSettings.startDistance);
                endDistance = EditorGUILayout.FloatField(endDistanceText, fogSettings.endDistance);
            } else {
                density = EditorGUILayout.FloatField(densityText, fogSettings.density);
            }
        } if (EditorGUI.EndChangeCheck()) {
            ApplyWindowSettingsToFogSettings(fogSettings);
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
        fogSettings = EnhancedFogLoader.GetFogSettings(scene);
        currentScene = scene;

        if (!fogSettings) {
            fogSettingsNotFoundOnDisk = true;
            fogSettings = ScriptableObject.CreateInstance(typeof(EnhancedFogSettings)) as EnhancedFogSettings;
            fogSettings.isEnabled = EnhancedFog.isEnabled;
            fogSettings.colorMode = EnhancedFog.colorMode;
            fogSettings.color = EnhancedFog.color;
            fogSettings.gradient = EnhancedFog.gradient;
            fogSettings.gradientTexture = EnhancedFog.gradientTexture;
            fogSettings.mode = EnhancedFog.mode;
            fogSettings.startDistance = EnhancedFog.startDistance;
            fogSettings.endDistance = EnhancedFog.endDistance;
            fogSettings.density = EnhancedFog.density;
        } else {
            fogSettingsNotFoundOnDisk = false;
        }

        isEnabled = fogSettings.isEnabled;
        colorMode = fogSettings.colorMode;
        color = fogSettings.color;
        gradient = fogSettings.gradient;
        gradientTexture = fogSettings.gradientTexture;
        mode = fogSettings.mode;
        startDistance = fogSettings.startDistance;
        endDistance = fogSettings.endDistance;
        density = fogSettings.density;

        unityLogo = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;
        string headerText = string.Join(blankspace, blankspace + scene.name, fogSettingsText);
        headerContent = new GUIContent(headerText, unityLogo, "");
    }

    private void OnUndoRedo() {
        if (fogSettings) {
            fogSettings.Render();
            SceneView.RepaintAll();
        }
    }

    private void DrawCreateFogSettingsAssetButton() {
        EditorGUILayout.HelpBox(helpBoxText, MessageType.Warning);
        if (GUILayout.Button("Create Fog Settings Asset")) {
            CreateFogSettingsAsset();
        }
        EditorGUILayout.Space();
    }

    private void CreateFogSettingsAsset() {
        EnhancedFogSettings newlyCreatedFogSettings = EnhancedFogLoader.CreateFogSettings(currentScene.path);
        ApplyWindowSettingsToFogSettings(newlyCreatedFogSettings);

        if (newlyCreatedFogSettings != null) {
            Initialize(currentScene);
        }
    }

    private void ApplyWindowSettingsToFogSettings(EnhancedFogSettings fogSettings) {
        Undo.RecordObject(fogSettings, "Changed Enhanced Fog Settings");
        fogSettings.isEnabled = isEnabled;
        fogSettings.colorMode = colorMode;
        fogSettings.color = color;
        fogSettings.gradient = gradient;
        fogSettings.gradientTexture = gradientTexture;
        fogSettings.mode = mode;
        fogSettings.startDistance = startDistance;
        fogSettings.endDistance = endDistance;
        fogSettings.density = density;
        fogSettings.Render();
        SceneView.RepaintAll();
        EditorUtility.SetDirty(fogSettings);
        AssetDatabase.SaveAssets();
    }

    private Texture2D GenerateGradientTexture(Gradient gradient, int textureWidth) {
        Texture2D gradientTexture = new Texture2D(textureWidth, 1, TextureFormat.ARGB32, false );
        gradientTexture.wrapMode = TextureWrapMode.Clamp;

        for (int i = 0; i < textureWidth; i++) {
            float progress = i / (textureWidth - 1.0f);
            gradientTexture.SetPixel(i, 0, gradient.Evaluate(progress));
        }

        gradientTexture.Apply();
        return gradientTexture;
    }
}
