using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] PlayerHealth playerHealth;

    void Awake()
    {
        // Disable this HUD for remote players' prefabs
        PhotonView pv = GetComponentInParent<PhotonView>();
        if (pv != null && !pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
    }

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
