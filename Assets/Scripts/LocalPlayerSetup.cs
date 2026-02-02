using Photon.Pun;
using UnityEngine;

public class LocalPlayerSetup : MonoBehaviour
{
    public Camera playerCamera;
    public MonoBehaviour[] localOnlyScripts;

    void Awake()
    {
        if (!GetComponent<PhotonView>().IsMine)
        {
            // Disable camera/audio
            if (playerCamera != null) playerCamera.enabled = false;
            AudioListener listener = GetComponentInChildren<AudioListener>();
            if (listener != null) listener.enabled = false;

            // Disable all local-only scripts
            foreach (var script in localOnlyScripts)
                script.enabled = false;
        }
    }
}
