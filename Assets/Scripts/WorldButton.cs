using UnityEngine;
using Photon.Pun;

public class WorldButton : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    public string sceneToLoad = "Map1";
    public float interactDistance = 3f;

    private Transform localPlayer;
    private bool playerInRange = false;

    void Update()
    {
        if (localPlayer == null)
        {
            FindLocalPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, localPlayer.position);
        playerInRange = distance <= interactDistance;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
            OnButtonPressed();
    }

    void FindLocalPlayer()
    {
        foreach (var pv in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
        {
            if (pv.IsMine && pv.CompareTag("Player"))
            {
                localPlayer = pv.transform;
                break;
            }
        }
    }

    void OnButtonPressed()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneToLoad);
        else
            photonView.RPC("LoadLevelRPC", RpcTarget.MasterClient, sceneToLoad);
    }

    [PunRPC]
    void LoadLevelRPC(string sceneName)
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneName);
    }
}