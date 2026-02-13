using UnityEngine;
using Photon.Pun;

public class ItemInteraction : MonoBehaviourPun
{

    [SerializeField] Animator animator;

    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    Camera cam;

    PhotonView view;

    void Awake()
    {   
        view = GetComponent<PhotonView>();
        if (!photonView.IsMine)        {
            // completely disable this script on remote players
            this.enabled = false;
        }
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
