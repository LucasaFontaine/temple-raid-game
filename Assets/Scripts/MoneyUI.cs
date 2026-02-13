using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] PlayerMoney playerMoney;

    void Start()
    {
        if (moneyText != null)
            moneyText.text = "0";
    }

    void OnEnable()
    {
        if (playerMoney == null)
        {
            // Assign local player's money automatically
            PlayerMoney localMoney = FindAnyObjectByType<PlayerMoney>();
            if (localMoney != null)
                playerMoney = localMoney;
        }

        if (playerMoney != null)
        {
            playerMoney.onMoneyChanged += UpdateMoney;
            UpdateMoney(playerMoney.money);
        }
    }

    void OnDisable()
    {
        if (playerMoney != null)
            playerMoney.onMoneyChanged -= UpdateMoney;
    }

    void UpdateMoney(int amount)
    {
        if (moneyText != null)
            moneyText.text = amount.ToString();
    }
}
