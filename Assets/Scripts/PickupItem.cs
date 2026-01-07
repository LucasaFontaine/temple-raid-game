using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int value = 10;

    public void Interact(GameObject player)
    {
        PlayerMoney money = player.GetComponent<PlayerMoney>();
        if (money != null)
        {
            money.AddMoney(value);
        }

        Destroy(gameObject);
    }
}
