using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public int damage = 10;

    void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Hit: " + other.name);
        }
    }
}
