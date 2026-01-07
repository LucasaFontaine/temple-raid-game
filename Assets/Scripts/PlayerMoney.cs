using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int money { get; private set; }

    public event System.Action<int> MoneyChanged;

    public void AddMoney(int amount)
    {
        money += amount;
        MoneyChanged?.Invoke(money);
    }
}
