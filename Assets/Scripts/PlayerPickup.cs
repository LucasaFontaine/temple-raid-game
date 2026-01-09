using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float interactDistance = 3f;
    public LayerMask interactLayer;
    public Transform holdPoint;

    Camera cam;

    PickupObject heldObject;
    public bool HasKey { get; private set; }

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();

        if (Input.GetKeyDown(KeyCode.G))
            Drop();
    }

    void TryInteract()
    {
        if (!Physics.Raycast(
            cam.transform.position,
            cam.transform.forward,
            out RaycastHit hit,
            interactDistance,
            interactLayer))
            return;

        Debug.Log("Hit: " + hit.collider.name);

        ChestInteractable chest =
            hit.collider.GetComponentInParent<ChestInteractable>();

        if (chest != null)
        {
            chest.TryOpen(HasKey);
            return;
        }

        // ----- PICKUP OBJECT -----
        if (heldObject != null) return;

        PickupObject pickup = hit.collider.GetComponent<PickupObject>();
        if (pickup == null) return;

        PickUp(pickup);
    }

    void PickUp(PickupObject pickup)
    {
        heldObject = pickup;

        // Key check
        if (pickup.CompareTag("Key"))
            HasKey = true;

        // Disable physics
        pickup.rb.isKinematic = true;
        pickup.rb.useGravity = false;
        pickup.col.enabled = false;

        // Attach to hand
        pickup.transform.SetParent(holdPoint);
        pickup.transform.localPosition = Vector3.zero;
        pickup.transform.localRotation = Quaternion.identity;
    }

    void Drop()
    {
        if (heldObject == null) return;

        // Remove key if dropped
        if (heldObject.CompareTag("Key"))
            HasKey = false;

        heldObject.transform.SetParent(null);

        // Re-enable physics
        heldObject.rb.isKinematic = false;
        heldObject.rb.useGravity = true;
        heldObject.col.enabled = true;

        // Small forward toss
        heldObject.rb.linearVelocity = cam.transform.forward * 2f;

        heldObject = null;
    }
}
