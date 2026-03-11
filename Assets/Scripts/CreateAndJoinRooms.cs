using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;
    public TMP_InputField playerNameInput;

    [Header("Error Messages")]
    public GameObject createErrorMessage;
    public GameObject joinErrorMessage;

    public void CreateRoom()
    {
        if (string.IsNullOrWhiteSpace(createInput.text))
        {
            if (createErrorMessage != null)
                createErrorMessage.SetActive(true);
            return;
        }

        if (createErrorMessage != null)
            createErrorMessage.SetActive(false);

        PhotonNetwork.CreateRoom(createInput.text);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrWhiteSpace(joinInput.text))
        {
            if (joinErrorMessage != null)
                joinErrorMessage.SetActive(true);
            return;
        }

        if (joinErrorMessage != null)
            joinErrorMessage.SetActive(false);

        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = playerNameInput.text;
        PhotonNetwork.LoadLevel("Map1");
    }
}