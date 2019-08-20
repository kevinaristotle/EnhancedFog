using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnhancedFogDemo : MonoBehaviour {
    private static readonly string fogSettingsLabel = "<b>Fog Settings</b>";
    private static readonly string isFogEnabledLabel = "Enabled";
    private static readonly string colorRLabel = "Color R";
    private static readonly string colorGLabel = "Color G";
    private static readonly string colorBLabel = "Color B";
    private static readonly string modeLabel = "Mode";
    private static readonly string[] modes = { "Linear", "Exponential", "Exponential Squared" };
    private static readonly string densityLabel = "Density";
    private static readonly string startDistanceLabel = "Start Distance";
    private static readonly string endDistanceLabel = "End Distance";

    private float colorR;
    private float colorG;
    private float colorB;

    private int modeIndex;

    private string densityText;
    private string startDistanceText;
    private string endDistanceText;

    private void Start() {
        colorR = EnhancedFog.color.r;
        colorG = EnhancedFog.color.g;
        colorB = EnhancedFog.color.b;

        modeIndex = (int)EnhancedFog.mode;

        densityText = EnhancedFog.density.ToString();
        startDistanceText = EnhancedFog.startDistance.ToString();
        endDistanceText = EnhancedFog.endDistance.ToString();
    }

    private void OnGUI() {
        GUILayout.BeginVertical(); {
            GUILayout.Label(fogSettingsLabel);
            EnhancedFog.isEnabled = GUILayout.Toggle(EnhancedFog.isEnabled, isFogEnabledLabel);

            GUILayout.Label(colorRLabel);
            colorR = GUILayout.HorizontalSlider(colorR, 0.0f, 1.0f);
            GUILayout.Label(colorGLabel);
            colorG = GUILayout.HorizontalSlider(colorG, 0.0f, 1.0f);
            GUILayout.Label(colorBLabel);
            colorB = GUILayout.HorizontalSlider(colorB, 0.0f, 1.0f);
            EnhancedFog.color = new Color(colorR, colorG, colorB);

            modeIndex = GUILayout.SelectionGrid(modeIndex, modes, 3);
            EnhancedFog.mode = (EnhancedFogMode)modeIndex;

            switch(EnhancedFog.mode) {
                case EnhancedFogMode.Linear:
                    GUILayout.Label(startDistanceLabel);
                    startDistanceText = GUILayout.TextField(startDistanceText);
                    if (float.TryParse(startDistanceText, out float startDistanceTextToFloat)) {
                        EnhancedFog.startDistance = startDistanceTextToFloat;
                    }

                    GUILayout.Label(endDistanceLabel);
                    endDistanceText = GUILayout.TextField(endDistanceText);
                    if (float.TryParse(endDistanceText, out float endDistanceTextToFloat)) {
                        EnhancedFog.endDistance = endDistanceTextToFloat;
                    }
                    break;
                case EnhancedFogMode.Exponential:
                case EnhancedFogMode.ExponentialSquared:
                    GUILayout.Label(densityLabel);
                    densityText = GUILayout.TextField(densityText);
                    if (float.TryParse(densityText, out float densityTextToFloat)) {
                        EnhancedFog.density = densityTextToFloat;
                    }
                    break;
            }
        } GUILayout.EndVertical();
    }
}
