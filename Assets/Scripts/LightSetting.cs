using UnityEngine;
using UnityEngine.Rendering.Universal;
public class LightSetting : MonoBehaviour{
    public Light2D lightSource; // ライトの参照
    public float intensity = 1f; // ライトの強度

    void Start()
    {
        if (lightSource == null)
        {
            lightSource = GetComponent <Light2D>();
        }
        UpdateLightSettings();
    }

    void UpdateLightSettings()
    {
        if (lightSource != null)
        {
            lightSource.intensity = intensity;
        }
    }

    void OnValidate()
    {
        UpdateLightSettings();
    }
}