using TMPro;
using UnityEngine;
using Photon.Pun;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] PlayerMoney playerMoney;

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
        if (moneyText != null)
            moneyText.text = "0";
    }

    void OnEnable()
    {
        if (playerMoney == null)
        {
            // Find only the LOCAL player's PlayerMoney component
            foreach (var money in FindObjectsByType<PlayerMoney>(FindObjectsSortMode.None))
            {
                PhotonView pv = money.GetComponent<PhotonView>();
                if (pv != null && pv.IsMine)
                {
                    playerMoney = money;
                    break;
                }
            }
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
