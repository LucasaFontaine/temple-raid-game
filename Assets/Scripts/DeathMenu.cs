using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Collections;

public class DeathMenu : MonoBehaviour
{
    public GameObject container;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    [Header("Spectator")]
    public SpectatorCamera spectatorCamera;

    public bool isDead = false;

    void Awake()
    {
        PhotonView pv = GetComponentInParent<PhotonView>();
        if (pv != null && !pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Start()
    {
        container.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (spectatorCamera != null)
            spectatorCamera.StopSpectating();

        if (playerHealth != null)
            playerHealth.Died += OnPlayerDied;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (isDead) return;
        isDead = true;

        PlayerInputDisabled(true);

        if (playerLook != null)
            playerLook.LockRotation();

        // Disable first person camera
        if (playerLook != null && playerLook.playerCamera != null)
            playerLook.playerCamera.gameObject.SetActive(false);

        // Show death screen briefly then switch to spectator
        StartCoroutine(SpectateAfterDelay());
    }

    IEnumerator SpectateAfterDelay()
    {
        // Show death container for 2 seconds
        container.SetActive(true);

        if (EventSystem.current == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        yield return new WaitForSeconds(2f);

        // Hide death screen
        container.SetActive(false);

        // Lock cursor for spectating
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Start spectating
        if (spectatorCamera != null)
            spectatorCamera.StartSpectating();
    }

    private void PlayerInputDisabled(bool disabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = !disabled;

        if (playerLook != null)
            playerLook.canLook = !disabled;
    }
}