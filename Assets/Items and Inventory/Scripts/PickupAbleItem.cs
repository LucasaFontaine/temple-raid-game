using UnityEngine;

public class PickupableItem : Item
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private bool autoPickup = false;

    private Transform playerTransform;
    private bool playerInRange = false;

    private void Start()
    {
        // Find player - adjust tag as needed
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        playerInRange = distance <= pickupRange;

        if (playerInRange)
        {
            if (autoPickup)
            {
                TryPickup();
            }
            else if (Input.GetKeyDown(pickupKey))
            {
                TryPickup();
            }
        }
    }

    private void TryPickup()
    {
        if (InventoryManager.Instance != null)
        {
            bool success = InventoryManager.Instance.AddItem(this);
            if (success)
            {
                // Hide the item instead of destroying (so we can drop it later)
                gameObject.SetActive(false);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize pickup range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}