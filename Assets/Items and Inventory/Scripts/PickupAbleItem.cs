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
        // Only check for pickup on local player's client
        if (playerTransform == null || !gameObject.activeInHierarchy) return;

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
                // Request pickup from Master Client
                int viewID = photonView.ViewID;
                PhotonView.Find(viewID).RPC("RPC_RequestPickup", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }

    [PunRPC]
    private void RPC_RequestPickup(int playerActorNumber)
    {
        // Only Master Client executes this
        if (PhotonNetwork.IsMasterClient)
        {
            // Tell all clients to disable the item
            photonView.RPC("RPC_PickupItem", RpcTarget.AllBuffered, playerActorNumber);
        }
    }

    [PunRPC]
    private void RPC_PickupItem(int playerActorNumber)
    {
        // Disable on all clients
        gameObject.SetActive(false);
        Debug.Log($"Player {playerActorNumber} picked up {gameObject.name}");
    }

    [PunRPC]
    private void RPC_DropItem(Vector3 position, int playerActorNumber)
    {
        // Re-enable on all clients
        gameObject.SetActive(true);
        transform.position = position;
        Debug.Log($"Player {playerActorNumber} dropped {gameObject.name}");
    }

    [PunRPC]
    private void RPC_RequestDrop(Vector3 position, int playerActorNumber)
    {
        // Only Master Client executes this
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_DropItem", RpcTarget.AllBuffered, position, playerActorNumber);
        }
    }

    public void NetworkedDrop(Vector3 position)
    {
        int viewID = photonView.ViewID;
        PhotonView.Find(viewID).RPC("RPC_RequestDrop", RpcTarget.MasterClient, position, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}