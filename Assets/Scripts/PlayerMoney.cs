using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    [SerializeField] int startingMoney = 0;

    public int money { get; private set; }

    public event System.Action<int> MoneyChanged;

    void Awake()
    {
        money = startingMoney;
        MoneyChanged?.Invoke(money);
    }

    public void AddMoney(int amount)
    {
        money = Mathf.Max(0, money + amount);
        MoneyChanged?.Invoke(money);
    }
}
