using UnityEngine;
using System.Collections;

public class DenseVisibilityZone : MonoBehaviour
{
    [Header("Dense Settings")]
    public float denseFarClip = 30f;
    public float denseFogDensity = 0.15f;
    public Color denseFogColor = new Color(0.5f, 0.5f, 0.5f);
    public float transitionTime = 1.2f;

    Camera cam;
    Coroutine transitionRoutine;

    float originalFarClip;
    float originalFogDensity;
    Color originalFogColor;
    bool originalFogState;
    Material originalSkybox;

    bool inside;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        cam = other.GetComponentInChildren<Camera>();
        if (!cam) return;

        if (!inside)
            SaveOriginalValues();

        inside = true;

        StartTransition(
            cam.farClipPlane,
            denseFarClip,
            RenderSettings.fogDensity,
            denseFogDensity,
            RenderSettings.fogColor,
            denseFogColor
        );
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        inside = false;

        // RESTORE SKYBOX IMMEDIATELY
        RenderSettings.skybox = originalSkybox;

        StartTransition(
            cam.farClipPlane,
            originalFarClip,
            RenderSettings.fogDensity,
            originalFogDensity,
            RenderSettings.fogColor,
            originalFogColor,
            restoreFogState: true
        );
    }

    void SaveOriginalValues()
    {
        originalFarClip = cam.farClipPlane;
        originalFogDensity = RenderSettings.fogDensity;
        originalFogColor = RenderSettings.fogColor;
        originalFogState = RenderSettings.fog;
        originalSkybox = RenderSettings.skybox;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.skybox = null;
    }

    void StartTransition(
        float farFrom, float farTo,
        float fogFrom, float fogTo,
        Color colorFrom, Color colorTo,
        bool restoreFogState = false)
    {
        if (transitionRoutine != null)
            StopCoroutine(transitionRoutine);

        transitionRoutine = StartCoroutine(
            Transition(farFrom, farTo, fogFrom, fogTo, colorFrom, colorTo, restoreFogState)
        );
    }

    IEnumerator Transition(
        float farFrom, float farTo,
        float fogFrom, float fogTo,
        Color colorFrom, Color colorTo,
        bool restoreFogState)
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / transitionTime;

            cam.farClipPlane = Mathf.Lerp(farFrom, farTo, t);
            RenderSettings.fogDensity = Mathf.Lerp(fogFrom, fogTo, t);
            RenderSettings.fogColor = Color.Lerp(colorFrom, colorTo, t);

            yield return null;
        }

        if (restoreFogState)
            RenderSettings.fog = originalFogState;
    }
}
