using UnityEngine;

public class ItemInteraction : MonoBehaviour
{

    [SerializeField] Animator animator;

    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    Camera cam;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            animator.SetTrigger("Interact");
            TryInteract();
        }   
    }

    void TryInteract()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
        {
            MoneyPickup item = hit.collider.GetComponent<MoneyPickup>();

            if (item != null)
            {
                item.Interact(gameObject);
            }
        }

        Debug.DrawRay(cam.transform.position, cam.transform.forward * interactDistance, Color.green);
    }
}
