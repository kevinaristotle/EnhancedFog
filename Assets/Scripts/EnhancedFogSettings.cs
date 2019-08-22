using UnityEngine;

public class EnhancedFogSettings : ScriptableObject {
    public bool isEnabled = false;
    public Color color = Color.gray;
    public EnhancedFogMode mode = EnhancedFogMode.Linear;
    public float density = 0.01f;
    public float startDistance = 0.0f;
    public float endDistance = 100.0f;

    public void Render() {
        EnhancedFog.currentFogSettings = this;
    }
}
