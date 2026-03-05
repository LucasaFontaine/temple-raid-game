using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class PhotonDisconnector : MonoBehaviourPunCallbacks
{
    private static PhotonDisconnector instance;

    public static void DisconnectAndLoadMenu()
    {
        // If one already exists, use it
        if (instance == null)
        {
            GameObject go = new GameObject("PhotonDisconnector");
            DontDestroyOnLoad(go);
            instance = go.AddComponent<PhotonDisconnector>();
        }

        instance.StartDisconnect();
    }

    public void StartDisconnect()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        else if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();
        else
        {
            SceneManager.LoadScene("MainMenu");
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SceneManager.LoadScene("MainMenu");
        Destroy(gameObject);
        instance = null;
    }
}