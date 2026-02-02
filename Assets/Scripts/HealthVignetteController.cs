using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class HealthVignetteController : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Volume volume;

    [Header("Health Vignette")]
    [Range(0f, 1f)]
    public float maxHealthIntensity = 0.6f;

    [Header("Damage Flash")]
    [Range(0f, 1f)]
    public float damageFlashIntensity = 0.8f;
    public float flashFadeSpeed = 6f;

    public Color healthyColor = Color.black;
    public Color lowHealthColor = Color.red;

    private Vignette vignette;

    private float baseIntensity;     // From health
    private float flashIntensity;    // From damage flash
    private Color targetColor;

    private int lastHealth;

    void Awake()
    {
        if (!volume.profile.TryGet(out vignette))
        {
            Debug.LogError("Vignette not found on Volume!");
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        if (playerHealth != null)
        {
            lastHealth = playerHealth.currentHealth;
            playerHealth.HealthChanged += OnHealthChanged;
        }
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.HealthChanged -= OnHealthChanged;
    }

    void Update()
    {
        // Fade out damage flash
        flashIntensity = Mathf.Lerp(flashIntensity, 0f, Time.deltaTime * flashFadeSpeed);

        // Combine base health vignette + flash
        float finalIntensity = Mathf.Clamp01(baseIntensity + flashIntensity);
        vignette.intensity.value =
            Mathf.Lerp(vignette.intensity.value, finalIntensity, Time.deltaTime * 8f);

        vignette.color.value =
            Color.Lerp(vignette.color.value, targetColor, Time.deltaTime * 8f);
    }

    private void OnHealthChanged(int current, int max)
    {
        float healthPercent = (float)current / max;

        // Persistent vignette from low health
        baseIntensity = Mathf.Lerp(maxHealthIntensity, 0f, healthPercent);
        targetColor = Color.Lerp(lowHealthColor, healthyColor, healthPercent);

        // DAMAGE FLASH (only if health went DOWN)
        if (current < lastHealth)
        {
            flashIntensity = damageFlashIntensity;
        }

        lastHealth = current;
    }
}
