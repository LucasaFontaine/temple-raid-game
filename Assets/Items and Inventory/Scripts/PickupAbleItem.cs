using UnityEngine;
using Photon.Pun;

public class PickupableItem : Item
{
    [Header("Pickup Settings")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private KeyCode pickupKey = KeyCode.E;
    [SerializeField] private bool autoPickup = false;

    private Transform playerTransform;
    private bool playerInRange = false;
    private PhotonView photonView;

    // Tracked on all clients via RPC to prevent double-pickup race conditions
    private bool isPickedUp = false;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Find local player only
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                playerTransform = player.transform;
                break;
            }
        }
    }

    private void Update()
    {
        if (playerTransform == null || !gameObject.activeInHierarchy || isPickedUp) return;

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
        if (isPickedUp) return;

        // Check space locally before sending RPC to avoid unnecessary network traffic
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasSpace())
        {
            photonView.RPC("RPC_RequestPickup", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    private void RPC_RequestPickup(int playerActorNumber)
    {
        // MasterClient is the authority: only approve if not already picked up
        if (PhotonNetwork.IsMasterClient && !isPickedUp)
        {
            isPickedUp = true;
            photonView.RPC("RPC_PickupItem", RpcTarget.AllBuffered, playerActorNumber);
        }
    }

    [PunRPC]
    private void RPC_PickupItem(int playerActorNumber)
    {
        isPickedUp = true;

        // Only add to the picking player's inventory on their own client
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerActorNumber)
        {
            InventoryManager.Instance?.AddItem(this);
        }

        gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_RequestDrop(Vector3 position, int playerActorNumber)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            isPickedUp = false;
            photonView.RPC("RPC_DropItem", RpcTarget.AllBuffered, position, playerActorNumber);
        }
    }

    [PunRPC]
    private void RPC_DropItem(Vector3 position, int playerActorNumber)
    {
        isPickedUp = false;
        gameObject.SetActive(true);
        transform.position = position;
        Debug.Log($"Player {playerActorNumber} dropped {gameObject.name}");
    }

    public void NetworkedDrop(Vector3 position)
    {
        photonView.RPC("RPC_RequestDrop", RpcTarget.MasterClient, position, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
