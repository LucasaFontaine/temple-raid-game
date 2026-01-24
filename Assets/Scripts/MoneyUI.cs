using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI moneyText;
    [SerializeField] PlayerMoney playerMoney;

    void Start()
    {
        moneyText.text = "0";
    }

    void OnEnable()
    {
        playerMoney.MoneyChanged += UpdateMoney;
    }
    
    void UpdateMoney(int amount)
    {
        moneyText.text = amount.ToString();
    }
}
