using UnityEngine;
using Photon.Pun;

public class PlayerMoney : MonoBehaviourPun
{
    [SerializeField] private int currentMoney = 0;

    // Public property for easy access
    public int money => currentMoney;

    public delegate void OnMoneyChanged(int newAmount);
    public event OnMoneyChanged onMoneyChanged;

    [PunRPC]
    public void RPC_AddMoney(int amount)
    {
        // Only update money on the local player's client
        if (photonView.IsMine)
        {
            currentMoney += amount;
            onMoneyChanged?.Invoke(currentMoney);
            Debug.Log($"Money added: {amount}. Total: {currentMoney}");
        }
    }

    public void AddMoney(int amount)
    {
        if (photonView.IsMine)
        {
            currentMoney += amount;
            onMoneyChanged?.Invoke(currentMoney);
        }
    }

    public int GetMoney()
    {
        return currentMoney;
    }

    public bool SpendMoney(int amount)
    {
        if (photonView.IsMine && currentMoney >= amount)
        {
            currentMoney -= amount;
            onMoneyChanged?.Invoke(currentMoney);
            return true;
        }
        return false;
    }
}