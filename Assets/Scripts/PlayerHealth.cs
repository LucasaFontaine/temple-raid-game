using UnityEngine;
using Photon.Pun;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    [Header("Money")]
    [SerializeField] PlayerMoney playerMoney;

    public event System.Action<int, int> HealthChanged;
    public event System.Action Died;

    PhotonView photonView;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            enabled = false;
            return;
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (playerMoney != null)
        {
            int moneyLost = Random.Range(10, 31);
            playerMoney.AddMoney(-moneyLost);
        }

        if (currentHealth <= 0)
        {
            Died?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }
}
