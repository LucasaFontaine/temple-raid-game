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

    const string READY_KEY = "IsReady";

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
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable { { READY_KEY, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        if (!changedProps.ContainsKey(READY_KEY)) return;
        CheckAllReady();
    }

    void CheckAllReady()
    {
        if (countdownStarted) return;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (!player.CustomProperties.TryGetValue(READY_KEY, out isPlayerReady) || !(bool)isPlayerReady)
                return;
        }

        countdownStarted = true;
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        yield return new WaitForSeconds(countdownTime);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(sceneToLoad);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        CheckAllReady();
    }
}