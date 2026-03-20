using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

public class ReadyButton : MonoBehaviourPunCallbacks
{
    [Header("Settings")]
    public string sceneToLoad = "Map1";
    public float countdownTime = 3f;
    public float interactDistance = 3f;

    private Transform localPlayer;
    private bool isReady = false;
    private bool countdownStarted = false;

    private int readyCount = 0;

    void Update()
    {
        if (localPlayer == null)
        {
            FindLocalPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, localPlayer.position);

        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E) && !isReady)
            SetReady();
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

    void SetReady()
    {
        isReady = true;
        // Tell all clients this player is ready
        photonView.RPC("PlayerReadyRPC", RpcTarget.All);
    }

    [PunRPC]
    void PlayerReadyRPC()
    {
        readyCount++;
        Debug.Log("Ready count: " + readyCount + "/" + PhotonNetwork.PlayerList.Length);

        if (readyCount >= PhotonNetwork.PlayerList.Length && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(StartCountdown());
        }
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(countdownTime);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Recheck in case a player left while others were readying
        if (readyCount >= PhotonNetwork.PlayerList.Length && !countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(StartCountdown());
        }
    }
}