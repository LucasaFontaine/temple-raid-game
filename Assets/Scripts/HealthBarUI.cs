using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] PlayerHealth playerHealth;

    void Start()
    {
        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.currentHealth;
    }

    void OnEnable()
    {
        playerHealth.HealthChanged += UpdateHealth;
    }

    void OnDisable()
    {
        playerHealth.HealthChanged -= UpdateHealth;
    }

    void UpdateHealth(int current, int max)
    {
        slider.value = current;
    }
}
