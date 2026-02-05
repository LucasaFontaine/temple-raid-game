using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ShowPlayerNickName : MonoBehaviourPunCallbacks
{
    void Start()
    {
        GetComponent<Text>().text = photonView.Owner.NickName;
    }
}
