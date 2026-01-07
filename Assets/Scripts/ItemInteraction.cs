using UnityEngine;

public class ItemInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    Camera cam;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            PickupItem item = hit.collider.GetComponent<PickupItem>();

            if (item != null)
            {
                item.Interact(gameObject);
            }
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.green);
    }
}
