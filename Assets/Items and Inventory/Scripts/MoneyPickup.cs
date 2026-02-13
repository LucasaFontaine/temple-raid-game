using UnityEngine;
using Photon.Pun;

public class MoneyPickup : MonoBehaviourPun
{
    [Header("Pickup Settings")]
    public int value = 10;

    public void Interact(GameObject player)
    {
        // Only the local player should process this
        PhotonView playerView = player.GetComponent<PhotonView>();
        if (playerView != null && playerView.IsMine)
        {
            // Add money to local player
            PlayerMoney money = player.GetComponent<PlayerMoney>();
            if (money != null)
            {
                money.AddMoney(value);
            }

            // Request Master Client to destroy for everyone
            int viewID = photonView.ViewID;
            PhotonView.Find(viewID).RPC("RPC_RequestDestroy", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    private void RPC_RequestDestroy(int playerActorNumber)
    {
        // Only Master Client executes this
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_DestroyMoney", RpcTarget.AllBuffered, playerActorNumber);
        }
    }

    [PunRPC]
    private void RPC_DestroyMoney(int playerActorNumber)
    {
        // Disable on all clients (so late joiners don't see it)
        gameObject.SetActive(false);
        Debug.Log($"Player {playerActorNumber} picked up ${value}");
    }
}